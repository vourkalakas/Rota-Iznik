using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Core.Data;
using Core;

namespace Core.Http
{
    public class KeepAliveMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor
        public KeepAliveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods
        public async Task Invoke(HttpContext context, IWebYardımcısı webHelper)
        {
            if (DataAyarlarıYardımcısı.DatabaseYüklendi())
            {
                //keep alive page requested (we ignore it to prevent creating a guest customer records)
                var keepAliveUrl = $"{webHelper.SiteKonumuAl()}keepalive/index";
                if (webHelper.SayfanınUrlsiniAl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                    return;
            }
            //or call the next middleware in the request pipeline
            await _next(context);
        }
        
        #endregion
    }
}
