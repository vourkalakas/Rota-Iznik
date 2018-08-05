using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Core.Http.Uzantılar;
using Core.Altyapı;

namespace Services.KimlikDoğrulama.Harici
{
    public static partial class HariciYetkilendiriciYardımcısı
    {
        #region Constants
        private const string EXTERNAL_AUTHENTICATION_ERRORS = "rota.externalauth.errors";

        #endregion

        #region Methods
        
        public static void AddErrorsToDisplay(string error)
        {
            var session = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext.Session;
            var errors = session.Al<IList<string>>(EXTERNAL_AUTHENTICATION_ERRORS) ?? new List<string>();
            errors.Add(error);
            session.Ayarla(EXTERNAL_AUTHENTICATION_ERRORS, errors);
        }
        public static IList<string> RetrieveErrorsToDisplay()
        {
            var session = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext.Session;
            var errors = session.Al<IList<string>>(EXTERNAL_AUTHENTICATION_ERRORS);

            if (errors != null)
                session.Remove(EXTERNAL_AUTHENTICATION_ERRORS);

            return errors;
        }

        #endregion
    }
}