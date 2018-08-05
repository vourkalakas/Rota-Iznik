using System;
using System.Collections.Generic;
using System.Linq;
using Core.Önbellek;
using Core.Data;
using Core.Domain.Localization;
using Services.Yapılandırma;
using Services.Olaylar;
using Services.Siteler;

namespace Services.Localization
{
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        
        private const string LANGUAGES_BY_ID_KEY = "Rota.language.id-{0}";
        private const string LANGUAGES_ALL_KEY = "Rota.language.all-{0}";
        private const string LANGUAGES_PATTERN_KEY = "Rota.language.";

        #endregion

        #region Fields

        private readonly IDepo<Dil> _languageRepository;
        private readonly ISiteMappingServisi _siteMappingService;
        private readonly IStatikÖnbellekYönetici _cacheManager;
        private readonly IAyarlarServisi _settingService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IOlayYayınlayıcı _eventPublisher;

        #endregion

        #region Ctor
        public LanguageService(IStatikÖnbellekYönetici cacheManager,
            IDepo<Dil> languageRepository,
            ISiteMappingServisi siteMappingService,
            IAyarlarServisi settingService,
            LocalizationSettings localizationSettings,
            IOlayYayınlayıcı eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._languageRepository = languageRepository;
            this._siteMappingService = siteMappingService;
            this._settingService = settingService;
            this._localizationSettings = localizationSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion
        
        #region Methods
        public virtual void DeleteLanguage(Dil language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (language is IÖnbellekİçinVarlık)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            {
                foreach (var activeLanguage in GetAllLanguages())
                {
                    if (activeLanguage.Id != language.Id)
                    {
                        _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                        _settingService.AyarKaydet(_localizationSettings);
                        break;
                    }
                }
            }
            
            _languageRepository.Sil(language);
            _cacheManager.KalıpİleSil(LANGUAGES_PATTERN_KEY);
            _eventPublisher.OlaySilindi(language);
        }
        public virtual IList<Dil> GetAllLanguages(bool showHidden = false, int siteId = 0, bool loadCacheableCopy = true)
        {
            Func<IList<Dil>> loadLanguagesFunc = () =>
            {
                var query = _languageRepository.Tablo;
                if (!showHidden)
                    query = query.Where(l => l.Yayınlandı);
                query = query.OrderBy(l => l.GörüntülenmeSırası).ThenBy(l => l.Id);
                return query.ToList();
            };

            IList<Dil> languages;
            if (loadCacheableCopy)
            {
                //cacheable copy
                var key = string.Format(LANGUAGES_ALL_KEY, showHidden);
                languages = _cacheManager.Al(key, () =>
                {
                    var result = new List<Dil>();
                    foreach (var language in loadLanguagesFunc())
                        result.Add(new LanguageForCaching(language));
                    return result;
                });
            }
            else
            {
                languages = loadLanguagesFunc();
            }

            //store mapping
            if (siteId > 0)
            {
                languages = languages
                    .Where(l => _siteMappingService.YetkiVer(l, siteId))
                    .ToList();
            }
            return languages;
        }
        public virtual Dil GetLanguageById(int languageId, bool loadCacheableCopy = true)
        {
            if (languageId == 0)
                return null;

            Func<Dil> loadLanguageFunc = () =>
            {
                return _languageRepository.AlId(languageId);
            };

            if (loadCacheableCopy)
            {
                //cacheable copy
                var key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
                return _cacheManager.Al(key, () =>
                {
                    var language = loadLanguageFunc();
                    if (language == null)
                        return null;
                    return new LanguageForCaching(language);
                });
            }

            return loadLanguageFunc();
        }
        public virtual void InsertLanguage(Dil language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (language is IÖnbellekİçinVarlık)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _languageRepository.Ekle(language);
            _cacheManager.KalıpİleSil(LANGUAGES_PATTERN_KEY);
            _eventPublisher.OlayEklendi(language);
        }
        public virtual void UpdateLanguage(Dil language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (language is IÖnbellekİçinVarlık)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _languageRepository.Güncelle(language);
            _cacheManager.KalıpİleSil(LANGUAGES_PATTERN_KEY);
            _eventPublisher.OlayGüncellendi(language);
        }

        #endregion
    }
}
