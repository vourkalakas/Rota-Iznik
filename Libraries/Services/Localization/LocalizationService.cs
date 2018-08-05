using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Core;
using Core.Önbellek;
using Core.Data;
using Core.Domain.Genel;
using Core.Domain.Localization;
using Data;
using Services.Olaylar;
using Services.Logging;

namespace Services.Localization
{
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants
        
        private const string LOCALSTRINGRESOURCES_ALL_PUBLIC_KEY = "Rota.lsr.all.public-{0}";
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Rota.lsr.all-{0}";
        private const string LOCALSTRINGRESOURCES_ALL_ADMIN_KEY = "Rota.lsr.all.admin-{0}";
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "Rota.lsr.{0}-{1}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Rota.lsr.";
        private const string ADMIN_LOCALSTRINGRESOURCES_PATTERN = "Admin.";

        #endregion

        #region Fields

        private readonly IDepo<LocaleStringResource> _lsrRepository;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly IStatikÖnbellekYönetici _cacheManager;
        private readonly IDataSağlayıcı _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly GenelAyarlar _commonSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IOlayYayınlayıcı _eventPublisher;

        #endregion

        #region Ctor
        public LocalizationService(IStatikÖnbellekYönetici cacheManager,
            ILogger logger,
            IWorkContext workContext,
            IDepo<LocaleStringResource> lsrRepository,
            IDataSağlayıcı dataProvider,
            IDbContext dbContext,
            GenelAyarlar commonSettings,
            LocalizationSettings localizationSettings, 
            IOlayYayınlayıcı eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._logger = logger;
            this._workContext = workContext;
            this._lsrRepository = lsrRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._localizationSettings = localizationSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities
        protected virtual void InsertLocaleStringResources(IList<LocaleStringResource> resources)
        {
            if (resources == null)
                throw new ArgumentNullException(nameof(resources));

            _lsrRepository.Ekle(resources);
            _cacheManager.KalıpİleSil(LOCALSTRINGRESOURCES_PATTERN_KEY);
            foreach (var resource in resources)
            {
                _eventPublisher.OlayEklendi(resource);
            }
        }
        protected virtual void UpdateLocaleStringResources(IList<LocaleStringResource> resources)
        {
            if (resources == null)
                throw new ArgumentNullException(nameof(resources));
            
            _lsrRepository.Güncelle(resources);
            _cacheManager.KalıpİleSil(LOCALSTRINGRESOURCES_PATTERN_KEY);
            foreach (var resource in resources)
            {
                _eventPublisher.OlayGüncellendi(resource);
            }
        }

        private static Dictionary<string, KeyValuePair<int, string>> ResourceValuesToDictionary(IEnumerable<LocaleStringResource> locales)
        {
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var locale in locales)
            {
                var resourceName = locale.ResourceName.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceValue));
            }
            return dictionary;
        }

        #endregion

        #region Methods
        public virtual void DeleteLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException(nameof(localeStringResource));

            _lsrRepository.Sil(localeStringResource);
            _cacheManager.KalıpİleSil(LOCALSTRINGRESOURCES_PATTERN_KEY);
            _eventPublisher.OlaySilindi(localeStringResource);
        }
        public virtual LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            return _lsrRepository.AlId(localeStringResourceId);
        }
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName)
        {
            if (_workContext.MevcutDil != null)
                return GetLocaleStringResourceByName(resourceName, _workContext.MevcutDil.Id);

            return null;
        }
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true)
        {
            var query = from lsr in _lsrRepository.Tablo
                        orderby lsr.ResourceName
                        where lsr.LanguageId == languageId && lsr.ResourceName == resourceName
                        select lsr;
            var localeStringResource = query.FirstOrDefault();

            if (localeStringResource == null && logIfNotFound)
                _logger.Uyarı($"Resource string ({resourceName}) not found. Language ID = {languageId}");
            return localeStringResource;
        }
        public virtual IList<LocaleStringResource> GetAllResources(int languageId)
        {
            var query = from l in _lsrRepository.Tablo
                        orderby l.ResourceName
                        where l.LanguageId == languageId
                        select l;
            var locales = query.ToList();
            return locales;
        }
        public virtual void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException(nameof(localeStringResource));
            
            _lsrRepository.Ekle(localeStringResource);
            _cacheManager.KalıpİleSil(LOCALSTRINGRESOURCES_PATTERN_KEY);
            _eventPublisher.OlayEklendi(localeStringResource);
        }
        public virtual void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException(nameof(localeStringResource));

            _lsrRepository.Güncelle(localeStringResource);
            _cacheManager.KalıpİleSil(LOCALSTRINGRESOURCES_PATTERN_KEY);
            _eventPublisher.OlayGüncellendi(localeStringResource);
        }
        public virtual Dictionary<string, KeyValuePair<int,string>> GetAllResourceValues(int languageId, bool? loadPublicLocales)
        {
            var key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            
            if (!loadPublicLocales.HasValue || _cacheManager.Ayarlandı(key))
            {
                var rez = _cacheManager.Al(key, () =>
                {
                    var query = from l in _lsrRepository.Tabloİzlemesiz
                        orderby l.ResourceName
                        where l.LanguageId == languageId
                        select l;

                    return ResourceValuesToDictionary(query);
                });
                
                _cacheManager.Sil(string.Format(LOCALSTRINGRESOURCES_ALL_PUBLIC_KEY, languageId));
                _cacheManager.Sil(string.Format(LOCALSTRINGRESOURCES_ALL_ADMIN_KEY, languageId));

                return rez;
            }
            key = string.Format(loadPublicLocales.Value ? LOCALSTRINGRESOURCES_ALL_PUBLIC_KEY : LOCALSTRINGRESOURCES_ALL_ADMIN_KEY, languageId);

            return _cacheManager.Al(key, () =>
            {
                var query = from l in _lsrRepository.Tabloİzlemesiz
                            orderby l.ResourceName
                            where l.LanguageId == languageId
                            select l;
                query = loadPublicLocales.Value ? query.Where(r =>  !r.ResourceName.StartsWith(ADMIN_LOCALSTRINGRESOURCES_PATTERN)) : query.Where(r => r.ResourceName.StartsWith(ADMIN_LOCALSTRINGRESOURCES_PATTERN));
                return ResourceValuesToDictionary(query);
            });
        }
        public virtual string GetResource(string resourceKey)
        {
            if (_workContext.MevcutDil != null)
                return GetResource(resourceKey, _workContext.MevcutDil.Id);
            
            return "";
        }
        public virtual string GetResource(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records (we know they are cached)
                var resources = GetAllResourceValues(languageId, !resourceKey.StartsWith(ADMIN_LOCALSTRINGRESOURCES_PATTERN, StringComparison.InvariantCultureIgnoreCase));
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey].Value;
                }
            }
            else
            {
                //gradual loading
                var key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, languageId, resourceKey);
                var lsr = _cacheManager.Al(key, () =>
                {
                    var query = from l in _lsrRepository.Tablo
                                where l.ResourceName == resourceKey
                                && l.LanguageId == languageId
                                select l.ResourceValue;
                    return query.FirstOrDefault();
                });

                if (lsr != null) 
                    result = lsr;
            }
            if (string.IsNullOrEmpty(result))
            {
                if (logIfNotFound)
                    _logger.Uyarı($"Resource string ({resourceKey}) is not found. Language ID = {languageId}");
                
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    result = defaultValue;
                }
                else
                {
                    if (!returnEmptyIfNotFound)
                        result = resourceKey;
                }
            }
            return result;
        }
        public virtual string ExportResourcesToXml(Dil language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Language");
            xmlWriter.WriteAttributeString("Name", language.Adı);
            xmlWriter.WriteAttributeString("SupportedVersion", Sürüm.MevcutSürüm);

            var resources = GetAllResources(language.Id);
            foreach (var resource in resources)
            {
                xmlWriter.WriteStartElement("LocaleResource");
                xmlWriter.WriteAttributeString("Name", resource.ResourceName);
                xmlWriter.WriteElementString("Value", null, resource.ResourceValue);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }
        public virtual void ImportResourcesFromXml(Dil language, string xml, bool updateExistingResources = true)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (string.IsNullOrEmpty(xml))
                return;
            if (_commonSettings.StoredProcedureKullanDestekliyse && _dataProvider.StoredProceduredDestekli)
            {
                var inDoc = new XmlDocument();
                inDoc.LoadXml(xml);
                var sb = new StringBuilder();
                using (var xWriter = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    inDoc.Save(xWriter);
                    xWriter.Close();
                }
                var outDoc = new XmlDocument();
                outDoc.LoadXml(sb.ToString());
                xml = outDoc.OuterXml;

                //stored procedures are enabled and supported by the database.
                var pLanguageId = _dataProvider.ParametreAl();
                pLanguageId.ParameterName = "LanguageId";
                pLanguageId.Value = language.Id;
                pLanguageId.DbType = DbType.Int32;

                var pXmlPackage = _dataProvider.ParametreAl();
                pXmlPackage.ParameterName = "XmlPackage";
                pXmlPackage.Value = xml;
                pXmlPackage.DbType = DbType.Xml;

                var pUpdateExistingResources = _dataProvider.ParametreAl();
                pUpdateExistingResources.ParameterName = "UpdateExistingResources";
                pUpdateExistingResources.Value = updateExistingResources;
                pUpdateExistingResources.DbType = DbType.Boolean;

                //long-running query. specify timeout (600 seconds)
                _dbContext.SqlKomutunuÇalıştır("EXEC [LanguagePackImport] @LanguageId, @XmlPackage, @UpdateExistingResources", 
                    false, 600, pLanguageId, pXmlPackage, pUpdateExistingResources);
            }
            else
            {
                //stored procedures aren't supported
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                var nodes = xmlDoc.SelectNodes(@"//Language/LocaleResource");

                var existingResources = GetAllResources(language.Id);
                var newResources = new List<LocaleStringResource>();

                foreach (XmlNode node in nodes)
                {
                    var name = node.Attributes["Name"].InnerText.Trim();
                    var value = "";
                    var valueNode = node.SelectSingleNode("Value");
                    if (valueNode != null)
                        value = valueNode.InnerText;

                    if (string.IsNullOrEmpty(name))
                        continue;
                    
                    var resource = existingResources.FirstOrDefault(x => x.ResourceName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                    if (resource != null)
                    {
                        if (updateExistingResources)
                        {
                            resource.ResourceValue = value;
                        }
                    }
                    else
                    {
                        newResources.Add(
                            new LocaleStringResource
                            {
                                LanguageId = language.Id,
                                ResourceName = name,
                                ResourceValue = value
                            });
                    }
                }
                InsertLocaleStringResources(newResources);
                UpdateLocaleStringResources(existingResources);
            }

            //clear cache
            _cacheManager.KalıpİleSil(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        #endregion
    }
}
