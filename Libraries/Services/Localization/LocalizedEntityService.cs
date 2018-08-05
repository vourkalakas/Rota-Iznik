using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core;
using Core.Önbellek;
using Core.Data;
using Core.Domain.Localization;
using Data;

namespace Services.Localization
{
    public partial class LocalizedEntityService : ILocalizedEntityService
    {
        #region Constants
        
        private const string LOCALIZEDPROPERTY_KEY = "Rota.localizedproperty.value-{0}-{1}-{2}-{3}";
        private const string LOCALIZEDPROPERTY_ALL_KEY = "Rota.localizedproperty.all";
        private const string LOCALIZEDPROPERTY_PATTERN_KEY = "Rota.localizedproperty.";

        #endregion

        #region Fields

        private readonly IDepo<LocalizedProperty> _localizedPropertyRepository;
        private readonly IStatikÖnbellekYönetici _cacheManager;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor
        public LocalizedEntityService(IStatikÖnbellekYönetici cacheManager,
            IDepo<LocalizedProperty> localizedPropertyRepository,
            LocalizationSettings localizationSettings)
        {
            this._cacheManager = cacheManager;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities
        protected virtual IList<LocalizedProperty> GetLocalizedProperties(int entityId, string localeKeyGroup)
        {
            if (entityId == 0 || string.IsNullOrEmpty(localeKeyGroup))
                return new List<LocalizedProperty>();

            var query = from lp in _localizedPropertyRepository.Tablo
                        orderby lp.Id
                        where lp.EntityId == entityId &&
                              lp.LocaleKeyGroup == localeKeyGroup
                        select lp;
            var props = query.ToList();
            return props;
        }
        protected virtual IList<LocalizedPropertyForCaching> GetAllLocalizedPropertiesCached()
        {
            //cache
            var key = string.Format(LOCALIZEDPROPERTY_ALL_KEY);
            return _cacheManager.Al(key, () =>
            {
                var query = from lp in _localizedPropertyRepository.Tablo
                            select lp;
                var localizedProperties = query.ToList();
                var list = new List<LocalizedPropertyForCaching>();
                foreach (var lp in localizedProperties)
                {
                    var localizedPropertyForCaching = new LocalizedPropertyForCaching
                    {
                        Id = lp.Id,
                        EntityId = lp.EntityId,
                        LanguageId = lp.LanguageId,
                        LocaleKeyGroup = lp.LocaleKeyGroup,
                        LocaleKey = lp.LocaleKey,
                        LocaleValue = lp.LocaleValue
                    };
                    list.Add(localizedPropertyForCaching);
                }
                return list;
            });
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class LocalizedPropertyForCaching
        {
            public int Id { get; set; }
            public int EntityId { get; set; }
            public int LanguageId { get; set; }
            public string LocaleKeyGroup { get; set; }
            public string LocaleKey { get; set; }
            public string LocaleValue { get; set; }
        }

        #endregion

        #region Methods

        public virtual void DeleteLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException(nameof(localizedProperty));

            _localizedPropertyRepository.Sil(localizedProperty);

            //cache
            _cacheManager.KalıpİleSil(LOCALIZEDPROPERTY_PATTERN_KEY);
        }
        public virtual LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId)
        {
            if (localizedPropertyId == 0)
                return null;

            return _localizedPropertyRepository.AlId(localizedPropertyId);
        }
        public virtual string GetLocalizedValue(int languageId, int entityId, string localeKeyGroup, string localeKey)
        {
            if (_localizationSettings.LoadAllLocalizedPropertiesOnStartup)
            {
                var key = string.Format(LOCALIZEDPROPERTY_KEY, languageId, entityId, localeKeyGroup, localeKey);
                return _cacheManager.Al(key, () =>
                {
                    //load all records (we know they are cached)
                    var source = GetAllLocalizedPropertiesCached();
                    var query = from lp in source
                                where lp.LanguageId == languageId &&
                                lp.EntityId == entityId &&
                                lp.LocaleKeyGroup == localeKeyGroup &&
                                lp.LocaleKey == localeKey
                                select lp.LocaleValue;
                    var localeValue = query.FirstOrDefault();
                    //little hack here. nulls aren't cacheable so set it to ""
                    if (localeValue == null)
                        localeValue = "";
                    return localeValue;
                });

            }
            else
            {
                //gradual loading
                var key = string.Format(LOCALIZEDPROPERTY_KEY, languageId, entityId, localeKeyGroup, localeKey);
                return _cacheManager.Al(key, () =>
                {
                    var source = _localizedPropertyRepository.Tablo;
                    var query = from lp in source
                                where lp.LanguageId == languageId &&
                                lp.EntityId == entityId &&
                                lp.LocaleKeyGroup == localeKeyGroup &&
                                lp.LocaleKey == localeKey
                                select lp.LocaleValue;
                    var localeValue = query.FirstOrDefault();
                    //little hack here. nulls aren't cacheable so set it to ""
                    if (localeValue == null)
                        localeValue = "";
                    return localeValue;
                });
            }
        }
        public virtual void InsertLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException(nameof(localizedProperty));

            _localizedPropertyRepository.Ekle(localizedProperty);

            //cache
            _cacheManager.KalıpİleSil(LOCALIZEDPROPERTY_PATTERN_KEY);
        }
        public virtual void UpdateLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException(nameof(localizedProperty));

            _localizedPropertyRepository.Güncelle(localizedProperty);

            //cache
            _cacheManager.KalıpİleSil(LOCALIZEDPROPERTY_PATTERN_KEY);
        }
        public virtual void SaveLocalizedValue<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            int languageId) where T : TemelVarlık, ILocalizedEntity
        {
            SaveLocalizedValue<T, string>(entity, keySelector, localeValue, languageId);
        }
        public virtual void SaveLocalizedValue<T, TPropType>(T entity,
            Expression<Func<T, TPropType>> keySelector,
            TPropType localeValue,
            int languageId) where T : TemelVarlık, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (languageId == 0)
                throw new ArgumentOutOfRangeException("languageId", "Language ID should not be 0");

            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            //load localized value (check whether it's a cacheable entity. In such cases we load its original entity type)
            var localeKeyGroup = entity.GetUnproxiedEntityType().Name;
            var localeKey = propInfo.Name;

            var props = GetLocalizedProperties(entity.Id, localeKeyGroup);
            var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            var localeValueStr = GenelYardımcı.To<string>(localeValue);
            
            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(localeValueStr))
                {
                    //delete
                    DeleteLocalizedProperty(prop);
                }
                else
                {
                    //update
                    prop.LocaleValue = localeValueStr;
                    UpdateLocalizedProperty(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(localeValueStr))
                {
                    //insert
                    prop = new LocalizedProperty
                    {
                        EntityId = entity.Id,
                        LanguageId = languageId,
                        LocaleKey = localeKey,
                        LocaleKeyGroup = localeKeyGroup,
                        LocaleValue = localeValueStr
                    };
                    InsertLocalizedProperty(prop);
                }
            }
        }

        #endregion
    }
}