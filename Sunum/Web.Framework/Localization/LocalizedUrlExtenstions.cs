using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Core.Domain.Localization;
using Core.Altyapı;
using Services.Localization;

namespace Web.Framework.Localization
{
    public static class LocalizedUrlExtenstions
    {
        public static bool IsLocalizedUrl(this string url, PathString pathBase, bool isRawPath, out Dil language)
        {
            language = null;
            if (string.IsNullOrEmpty(url))
                return false;
            
            if (isRawPath)
                url = url.RemoveApplicationPathFromRawUrl(pathBase);
            
            var firstSegment = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(firstSegment))
                return false;

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            language = languageService.GetAllLanguages()
                .FirstOrDefault(urlLanguage => urlLanguage.ÖzelSeoKodu.Equals(firstSegment, StringComparison.InvariantCultureIgnoreCase));

            return language?.Yayınlandı ?? false;
        }
        public static string RemoveApplicationPathFromRawUrl(this string rawUrl, PathString pathBase)
        {
            new PathString(rawUrl).StartsWithSegments(pathBase, out PathString result);
            return WebUtility.UrlDecode(result);
        }
        public static string RemoveLanguageSeoCodeFromUrl(this string url, PathString pathBase, bool isRawPath)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (isRawPath)
                url = url.RemoveApplicationPathFromRawUrl(pathBase);

            url = url.TrimStart('/');
            var result = url.Contains('/') ? url.Substring(url.IndexOf('/')) : string.Empty;

            if (isRawPath)
                result = pathBase + result;

            return result;
        }
        public static string AddLanguageSeoCodeToUrl(this string url, PathString pathBase, bool isRawPath, Dil language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (isRawPath && !string.IsNullOrEmpty(url))
                url = url.RemoveApplicationPathFromRawUrl(pathBase);

            url = $"/{language.ÖzelSeoKodu}{url}";

            if (isRawPath)
                url = pathBase + url;

            return url;
        }
    }
}