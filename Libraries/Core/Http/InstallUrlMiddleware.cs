using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Core.Data;

namespace Core.Http
{
    public class InstallUrlMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor
        
        public InstallUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods
        
        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context, IWebYardımcısı webHelper)
        {
            //whether database is installed
            if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
            {
                var installUrl = $"{webHelper.SiteKonumuAl()}install";
                if (!webHelper.SayfanınUrlsiniAl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    //redirect
                    context.Response.Redirect(installUrl);
                    return;
                }
            }

            //or call the next middleware in the request pipeline
            await _next(context);
        }
        
        #endregion
    }
}
