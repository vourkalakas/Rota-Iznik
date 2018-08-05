using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;
using Core;
using Core.Yapılandırma;
using Core.Altyapı;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Services.Yardımcılar
{
    public partial class KullanıcıAracıYardımcısı : IKullanıcıAracıYardımcısı
    {
        private readonly Config _ayar;
        private readonly IHttpContextAccessor _httpContext;
        private static readonly object _kilitleyici = new object();
        public KullanıcıAracıYardımcısı(Config ayar, IHttpContextAccessor httpContext)
        {
            this._ayar = ayar;
            this._httpContext = httpContext;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual BrowscapXmlHelper GetBrowscapXmlHelper()
        {
            if (Singleton<BrowscapXmlHelper>.Instance != null)
                return Singleton<BrowscapXmlHelper>.Instance;

            //no database created
            if (String.IsNullOrEmpty(_ayar.UserAgentStringsPath))
                return null;

            //prevent multi loading data
            lock (_kilitleyici)
            {
                //data can be loaded while we waited
                if (Singleton<BrowscapXmlHelper>.Instance != null)
                    return Singleton<BrowscapXmlHelper>.Instance;

                var userAgentStringsPath = GenelYardımcı.MapPath(_ayar.UserAgentStringsPath);
                var crawlerOnlyUserAgentStringsPath = string.IsNullOrEmpty(_ayar.CrawlerOnlyUserAgentStringsPath) ? string.Empty : GenelYardımcı.MapPath(_ayar.CrawlerOnlyUserAgentStringsPath);

                var browscapXmlHelper = new BrowscapXmlHelper(userAgentStringsPath, crawlerOnlyUserAgentStringsPath);
                Singleton<BrowscapXmlHelper>.Instance = browscapXmlHelper;

                return Singleton<BrowscapXmlHelper>.Instance;
            }
        }
        public virtual bool AramaMotoru()
        {
            if (_httpContext == null)
                return false;

            //we put required logic in try-catch block
            //more info: http://www.nopcommerce.com/boards/t/17711/unhandled-exception-request-is-not-available-in-this-context.aspx
            try
            {
                var bowscapXmlHelper = GetBrowscapXmlHelper();

                //we cannot load parser
                if (bowscapXmlHelper == null)
                    return false;

                var userAgent = _httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent];
                return !string.IsNullOrWhiteSpace(userAgent) && bowscapXmlHelper.IsCrawler(userAgent);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return false;
        }
    }
}

