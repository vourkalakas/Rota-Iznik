using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Core.Data;
using Core.Domain.Localization;
using Core.Altyapı;

namespace Web.Framework.Localization
{
    public class LocalizedRoute : Route
    {
        #region Fields

        private readonly IRouter _target;
        private bool? _seoFriendlyUrlsForLanguagesEnabled;

        #endregion

        #region Ctor
        public LocalizedRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, 
            IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        #endregion

        #region Methods
        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            var data = base.GetVirtualPath(context);
            if (data == null)
                return null;

            if (!DataAyarlarıYardımcısı.DatabaseYüklendi() || !SeoFriendlyUrlsForLanguagesEnabled)
                return data;
            
            var path = context.HttpContext.Request.Path.Value;
            if (path.IsLocalizedUrl(context.HttpContext.Request.PathBase, false, out Dil language))
                data.VirtualPath = $"/{language.ÖzelSeoKodu}{data.VirtualPath}";

            return data;
        }
        public override Task RouteAsync(RouteContext context)
        {
            if (!DataAyarlarıYardımcısı.DatabaseYüklendi() || !SeoFriendlyUrlsForLanguagesEnabled)
                return base.RouteAsync(context);
            var path = context.HttpContext.Request.Path.Value;
            if (!path.IsLocalizedUrl(context.HttpContext.Request.PathBase, false, out Dil _))
                return base.RouteAsync(context);

            var newPath = path.RemoveLanguageSeoCodeFromUrl(context.HttpContext.Request.PathBase, false);

            context.HttpContext.Request.Path = newPath;
            base.RouteAsync(context).Wait();

            context.HttpContext.Request.Path = path;
            return _target.RouteAsync(context);
        }
        public virtual void ClearSeoFriendlyUrlsCachedValue()
        {
            _seoFriendlyUrlsForLanguagesEnabled = null;
        }
        
        #endregion
        
        #region Properties
        protected bool SeoFriendlyUrlsForLanguagesEnabled
        {
            get
            {
                if (!_seoFriendlyUrlsForLanguagesEnabled.HasValue)
                    _seoFriendlyUrlsForLanguagesEnabled = EngineContext.Current.Resolve<LocalizationSettings>().SeoFriendlyUrlsForLanguagesEnabled;

                return _seoFriendlyUrlsForLanguagesEnabled.Value;
            }
        }
        
        #endregion
    }
}