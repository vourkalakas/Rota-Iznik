using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core;
using Core.Data;
using Services.Kullanıcılar;

namespace Web.Framework.Mvc.Filtreler
{
    public class SonİşlemAttribute : TypeFilterAttribute
    {
        #region Ctor
        public SonİşlemAttribute() : base(typeof(SonİşlemKaydetFilter))
        {
        }

        #endregion

        #region Nested filter
        private class SonİşlemKaydetFilter : IActionFilter
        {
            #region Fields

            private readonly IKullanıcıServisi _kullanıcıServisi;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public SonİşlemKaydetFilter(IKullanıcıServisi kullanıcıServisi,
                IWorkContext workContext)
            {
                this._kullanıcıServisi = kullanıcıServisi;
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

                //update last activity date
                if (_workContext.MevcutKullanıcı.SonİşlemTarihi.AddMinutes(1.0) < DateTime.UtcNow)
                {
                    _workContext.MevcutKullanıcı.SonİşlemTarihi = DateTime.UtcNow;
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
