using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core;
using Core.Data;
using Core.Domain.Kullanıcılar;
using Services.Kullanıcılar;

namespace Web.Framework.Mvc.Filtreler
{
    public class SiteIpAdresiAttribute : TypeFilterAttribute
    {
        #region Ctor
        
        public SiteIpAdresiAttribute() : base(typeof(IpAdresiKaydetFilter))
        {
        }

        #endregion

        #region Nested filter
        private class IpAdresiKaydetFilter : IActionFilter
        {
            #region Fields

            private readonly IKullanıcıServisi _kullanıcıServisi;
            private readonly IWebYardımcısı _webHelper;
            private readonly IWorkContext _workContext;
            private readonly KullanıcıAyarları _kullanıcıAyarları;

            #endregion

            #region Ctor

            public IpAdresiKaydetFilter(IKullanıcıServisi kullanıcıServisi,
                IWebYardımcısı webHelper,
                IWorkContext workContext,
                KullanıcıAyarları kullanıcıAyarları)
            {
                this._kullanıcıServisi = kullanıcıServisi;
                this._webHelper = webHelper;
                this._workContext = workContext;
                this._kullanıcıAyarları = kullanıcıAyarları;
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

                var currentIpAddress = _webHelper.MevcutIpAdresiAl();
                if (string.IsNullOrEmpty(currentIpAddress))
                    return;

                if (!currentIpAddress.Equals(_workContext.MevcutKullanıcı.SonIPAdresi, StringComparison.InvariantCultureIgnoreCase))
                {
                    _workContext.MevcutKullanıcı.SonIPAdresi = currentIpAddress;
                    _kullanıcıServisi.KullanıcıGüncelle(_workContext.MevcutKullanıcı);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }

            #endregion
        }

        #endregion
    }
}
