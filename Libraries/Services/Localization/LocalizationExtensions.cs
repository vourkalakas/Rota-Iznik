using System;
using System.Linq.Expressions;
using System.Reflection;
using Core;
using Core.Yapılandırma;
using Core.Domain.Localization;
using Core.Domain.Güvenlik;
using Core.Altyapı;
using Core.Eklentiler;
using Data;
using Services.Yapılandırma;

namespace Services.Localization
{
    public static class LocalizationExtensions
    {
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector)
            where T : TemelVarlık, ILocalizedEntity
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return GetLocalized(entity, keySelector, workContext.MevcutDil.Id);
        }
        public static string GetLocalized<T>(this T entity, 
            Expression<Func<T, string>> keySelector, int languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true) 
            where T : TemelVarlık, ILocalizedEntity
        {
            return GetLocalized<T, string>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }
        public static TPropType GetLocalized<T, TPropType>(this T entity,
            Expression<Func<T, TPropType>> keySelector, int languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : TemelVarlık, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");
            }

            var result = default(TPropType);
            var resultStr = string.Empty;

            var localeKeyGroup = entity.GetUnproxiedEntityType().Name;
            var localeKey = propInfo.Name;

            if (languageId > 0)
            {
                //ensure that we have at least two published languages
                var loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var lService = EngineContext.Current.Resolve<ILanguageService>();
                    var totalPublishedLanguages = lService.GetAllLanguages().Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    var leService = EngineContext.Current.Resolve<ILocalizedEntityService>();
                    resultStr = leService.GetLocalizedValue(languageId, entity.Id, localeKeyGroup, localeKey);
                    if (!string.IsNullOrEmpty(resultStr))
                        result = GenelYardımcı.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (string.IsNullOrEmpty(resultStr) && returnDefaultValue)
            {
                var localizer = keySelector.Compile();
                result = localizer(entity);
            }
            
            return result;
        }
        public static string GetLocalizedSetting<T>(this T settings,
            Expression<Func<T, string>> keySelector, int languageId, int storeId,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : IAyarlar, new()
        {
            var settingService = EngineContext.Current.Resolve<IAyarlarServisi>();

            var key = settings.GetSettingKey(keySelector);
            var setting = settingService.AyarAl(key, siteId: storeId, paylaşılanDeğerYoksaYükle: true);
            if (setting == null)
                return null;

            return setting.GetLocalized(x => x.Değer, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }
        public static void SaveLocalizedSetting<T>(this T settings,
            Expression<Func<T, string>> keySelector, int languageId,
            string value)
            where T : IAyarlar, new()
        {
            var settingService = EngineContext.Current.Resolve<IAyarlarServisi>();
            var localizedEntityService = EngineContext.Current.Resolve<ILocalizedEntityService>();

            var key = settings.GetSettingKey(keySelector);
            
            var setting = settingService.AyarAl(key, siteId: 0, paylaşılanDeğerYoksaYükle: false);
            if (setting == null)
                return;

            localizedEntityService.SaveLocalizedValue(setting, x => x.Değer, value, languageId);
        }
        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, IWorkContext workContext)
            where T : struct
        {
            if (workContext == null)
                throw new ArgumentNullException(nameof(workContext));

            return GetLocalizedEnum(enumValue, localizationService, workContext.MevcutDil.Id);
        }
        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, int languageId)
            where T : struct
        {
            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));

            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            //localized value
            var resourceName = $"Enums.{typeof(T)}.{enumValue}";
            var result = localizationService.GetResource(resourceName, languageId, false, "", true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = GenelYardımcı.EnumDönüştür(enumValue.ToString());

            return result;
        }
        public static string GetLocalizedPermissionName(this İzinKaydı permissionRecord,
            ILocalizationService localizationService, IWorkContext workContext)
        {
            if (workContext == null)
                throw new ArgumentNullException(nameof(workContext));

            return GetLocalizedPermissionName(permissionRecord, localizationService, workContext.MevcutDil.Id);
        }
        public static string GetLocalizedPermissionName(this İzinKaydı permissionRecord, 
            ILocalizationService localizationService, int languageId)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException(nameof(permissionRecord));

            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));

            //localized value
            var resourceName = $"Permission.{permissionRecord.SistemAdı}";
            var result = localizationService.GetResource(resourceName, languageId, false, "", true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = permissionRecord.Adı;

            return result;
        }
        public static void SaveLocalizedPermissionName(this İzinKaydı permissionRecord,
            ILocalizationService localizationService, ILanguageService languageService)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException(nameof(permissionRecord));
            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));
            if (languageService == null)
                throw new ArgumentNullException(nameof(languageService));

            var resourceName = $"Permission.{permissionRecord.SistemAdı}";
            var resourceValue = permissionRecord.Adı;

            foreach (var lang in languageService.GetAllLanguages(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    localizationService.InsertLocaleStringResource(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    localizationService.UpdateLocaleStringResource(lsr);
                }
            }
        }
        public static void DeleteLocalizedPermissionName(this İzinKaydı permissionRecord,
            ILocalizationService localizationService, ILanguageService languageService)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException(nameof(permissionRecord));
            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));
            if (languageService == null)
                throw new ArgumentNullException(nameof(languageService));

            var resourceName = $"Permission.{permissionRecord.SistemAdı}";
            foreach (var lang in languageService.GetAllLanguages(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr != null)
                    localizationService.DeleteLocaleStringResource(lsr);
            }
        }
        public static void DeletePluginLocaleResource(this TemelEklenti plugin,
            string resourceName)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            DeletePluginLocaleResource(plugin, localizationService,
                languageService, resourceName);
        }
        public static void DeletePluginLocaleResource(this TemelEklenti plugin,
            ILocalizationService localizationService, ILanguageService languageService,
            string resourceName)
        {
            //actually plugin instance is not required
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));
            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));
            if (languageService == null)
                throw new ArgumentNullException(nameof(languageService));

            foreach (var lang in languageService.GetAllLanguages(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr != null)
                    localizationService.DeleteLocaleStringResource(lsr);
            }
        }
        public static void AddOrUpdatePluginLocaleResource(this TemelEklenti plugin,
            string resourceName, string resourceValue, string languageCulture = null)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
             AddOrUpdatePluginLocaleResource(plugin, localizationService,
                 languageService, resourceName, resourceValue, languageCulture);
        }
        public static void AddOrUpdatePluginLocaleResource(this TemelEklenti plugin, 
            ILocalizationService localizationService, ILanguageService languageService, 
            string resourceName, string resourceValue, string languageCulture = null)
        {
            //actually plugin instance is not required
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));
            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));
            if (languageService == null)
                throw new ArgumentNullException(nameof(languageService));
            
            foreach (var lang in languageService.GetAllLanguages(true))
            {
                if (!string.IsNullOrEmpty(languageCulture) && !languageCulture.Equals(lang.DilKültürü))
                    continue;

                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    localizationService.InsertLocaleStringResource(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    localizationService.UpdateLocaleStringResource(lsr);
                }
            }
        }
        public static string GetLocalizedFriendlyName<T>(this T plugin, ILocalizationService localizationService, 
            int languageId, bool returnDefaultValue = true)
            where T : IEklenti
        {   
            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));

            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            if (plugin.EklentiTanımlayıcı == null)
                throw new ArgumentException("Plugin descriptor cannot be loaded");

            var systemName = plugin.EklentiTanımlayıcı.SistemAdı;
            //localized value
            var resourceName = $"Plugins.FriendlyName.{systemName}";
            var result = localizationService.GetResource(resourceName, languageId, false, "", true);

            //set default value if required
            if (string.IsNullOrEmpty(result) && returnDefaultValue)
                result = plugin.EklentiTanımlayıcı.KısaAd;

            return result;
        }
        public static void SaveLocalizedFriendlyName<T>(this T plugin, 
            ILocalizationService localizationService, int languageId,
            string localizedFriendlyName)
            where T : IEklenti
        {
            if (localizationService == null)
                throw new ArgumentNullException(nameof(localizationService));

            if (languageId == 0)
                throw new ArgumentOutOfRangeException("languageId", "Language ID should not be 0");

            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            if (plugin.EklentiTanımlayıcı == null)
                throw new ArgumentException("Plugin descriptor cannot be loaded");

            var systemName = plugin.EklentiTanımlayıcı.SistemAdı;
            //localized value
            var resourceName = $"Plugins.FriendlyName.{systemName}";
            var resource = localizationService.GetLocaleStringResourceByName(resourceName, languageId, false);

            if (resource != null)
            {
                if (string.IsNullOrWhiteSpace(localizedFriendlyName))
                {
                    //delete
                    localizationService.DeleteLocaleStringResource(resource);
                }
                else
                {
                    //update
                    resource.ResourceValue = localizedFriendlyName;
                    localizationService.UpdateLocaleStringResource(resource);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(localizedFriendlyName))
                    return;

                //insert
                resource = new LocaleStringResource
                {
                    LanguageId = languageId,
                    ResourceName = resourceName,
                    ResourceValue = localizedFriendlyName,
                };
                localizationService.InsertLocaleStringResource(resource);
            }
        }
    }
}
