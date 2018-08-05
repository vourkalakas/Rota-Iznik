using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Core;
using Core.Data;
using Services.Kullanıcılar;

namespace Web.Framework.Mvc.Filtreler
{
    public class ParolaDoğrulaAttribute : TypeFilterAttribute
    {
        #region Ctor
        public ParolaDoğrulaAttribute() : base(typeof(ParolaDoğrulaFilter))
        {
        }

        #endregion

        #region Nested filter
        private class ParolaDoğrulaFilter : IActionFilter
        {
            #region Fields

            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public ParolaDoğrulaFilter(IUrlHelperFactory urlHelperFactory,
                IWorkContext workContext)
            {
                this._urlHelperFactory = urlHelperFactory;
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

                if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
                    return;

                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;

                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;

                //don't validate on ChangePassword page
                if (!(controllerName.Equals("Kullanıcı", StringComparison.InvariantCultureIgnoreCase) &&
                    actionName.Equals("ParolaDeğiştir", StringComparison.InvariantCultureIgnoreCase)))
                {
                    //check password expiration
                    if (_workContext.MevcutKullanıcı.ŞifreKurtarmaLinkiSüresiDoldu())
                    {
                        var changePasswordUrl = _urlHelperFactory.GetUrlHelper(context).RouteUrl("KullanıcıParolaDeğiştirdi");
                        context.Result = new RedirectResult(changePasswordUrl);
                    }
                }
            }
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}
