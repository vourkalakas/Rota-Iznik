using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Core;
using Core.Domain.Siteler;
using Services.Siteler;

namespace Web.Framework
{
    public partial class WebSiteContext : ISiteContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISiteServisi _siteService;

        private Site _önbelleklenmişSite;
        public WebSiteContext(IHttpContextAccessor httpContextAccessor,
            ISiteServisi siteService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._siteService = siteService;
        }
        public virtual Site MevcutSite
        {
            get
            {
                if (_önbelleklenmişSite != null)
                    return _önbelleklenmişSite;
                
                string host = _httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Host];

                var allStores = _siteService.TümSiteler();
                var store = allStores.FirstOrDefault(s => s.HostDeğeriİçerir(host));

                if (store == null)
                {
                    store = allStores.FirstOrDefault();
                }

                _önbelleklenmişSite = store ?? throw new Exception("Site yüklenemedi");

                return _önbelleklenmişSite;
            }
        }
    }
}
