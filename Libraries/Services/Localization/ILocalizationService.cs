using System.Collections.Generic;
using Core.Domain.Localization;

namespace Services.Localization
{
    public partial interface ILocalizationService
    {
        void DeleteLocaleStringResource(LocaleStringResource localeStringResource);
        LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId);
        LocaleStringResource GetLocaleStringResourceByName(string resourceName);
        LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true);
        IList<LocaleStringResource> GetAllResources(int languageId);
        void InsertLocaleStringResource(LocaleStringResource localeStringResource);
        void UpdateLocaleStringResource(LocaleStringResource localeStringResource);
        Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId, bool? loadPublicLocales);
        string GetResource(string resourceKey);
        string GetResource(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);
        string ExportResourcesToXml(Dil language);
        void ImportResourcesFromXml(Dil language, string xml, bool updateExistingResources = true);
    }
}
