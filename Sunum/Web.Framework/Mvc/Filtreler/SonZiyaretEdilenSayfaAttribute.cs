using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core;
using Core.Data;
using Core.Domain.Kullanıcılar;
using Services.Genel;

namespace Web.Framework.Mvc.Filtreler
{
    public class SonZiyaretEdilenSayfaAttribute : TypeFilterAttribute
    {
        #region Ctor
        public SonZiyaretEdilenSayfaAttribute() : base(typeof(SonZiyaretEdilenSayfaFilter))
        {
        }
        #endregion

        #region Nested filter
        private class SonZiyaretEdilenSayfaFilter : IActionFilter
        {
            #region Fields

            private readonly KullanıcıAyarları _kullanıcıAyarları;
            private readonly IGenelÖznitelikServisi _genelÖznitelikServisi;
            private readonly IWebYardımcısı _webHelper;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public SonZiyaretEdilenSayfaFilter(KullanıcıAyarları kullanıcıAyarları,
                IGenelÖznitelikServisi genelÖznitelikServisi,
                IWebYardımcısı webHelper,
                IWorkContext workContext)
            {
                this._kullanıcıAyarları = kullanıcıAyarları;
                this._genelÖznitelikServisi = genelÖznitelikServisi;
                this._webHelper = webHelper;
                this._workContext = workContext;
            }

            #endregion

            #region Methods
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
                    return;

                if (!_kullanıcıAyarları.SiteSonZiyaretSayfası)
                    return;

                var pageUrl = _webHelper.SayfanınUrlsiniAl(true);
                if (string.IsNullOrEmpty(pageUrl))
                    return;

                var previousPageUrl = _workContext.MevcutKullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.SonZiyaretEdilenSayfa);

                if (!pageUrl.Equals(previousPageUrl, StringComparison.InvariantCultureIgnoreCase))
                    _genelÖznitelikServisi.ÖznitelikKaydet(_workContext.MevcutKullanıcı, SistemKullanıcıÖznitelikAdları.SonZiyaretEdilenSayfa, pageUrl);

            }
            public void OnActionExecuted(ActionExecutedContext context)
            {
            }

            #endregion
        }

        #endregion
    }
}
