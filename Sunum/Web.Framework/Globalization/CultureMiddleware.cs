using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Core;
using Core.Data;

namespace Web.Framework.Globalization
{
    public class CultureMiddleware
    {
        #region Fields
        private readonly RequestDelegate _next;
        #endregion

        #region Ctor
        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion

        #region Utilities
        protected void SetWorkingCulture(IWebYardımcısı webHelper, IWorkContext workContext)
        {
            if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
                return;

            var adminAreaUrl = $"{webHelper.SiteKonumuAl()}admin";
            if (webHelper.SayfanınUrlsiniAl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                GenelYardımcı.TelerikKültürAyarla();
                workContext.Yönetici = true;
            }
            else
            {
                var culture = new CultureInfo(workContext.MevcutDil.DilKültürü);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }
        #endregion

        #region Methods
        public Task Invoke(Microsoft.AspNetCore.Http.HttpContext context, IWebYardımcısı webHelper, IWorkContext workContext)
        {
            SetWorkingCulture(webHelper, workContext);
            return _next(context);
        }
        #endregion
    }
}
