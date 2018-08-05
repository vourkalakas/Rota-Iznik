using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Core.Altyapı;
using Core.Eklentiler;

namespace Web.Framework.Mvc.Routing
{
    public class RotaYayınlayıcı : IRotaYayınlayıcı
    {
        #region Fields
        protected readonly ITypeFinder typeFinder;
        #endregion

        #region Ctor
        public RotaYayınlayıcı(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }
        #endregion

        #region Methods
        public virtual void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            var routeProviders = typeFinder.FindClassesOfType<IRotaSağlayıcı>();

            var instances = routeProviders
                .Where(routeProvider => EklentiYönetici.EklentiBul(routeProvider)?.Kuruldu ?? true) //ignore not installed plugins
                .Select(routeProvider => (IRotaSağlayıcı)Activator.CreateInstance(routeProvider))
                .OrderByDescending(routeProvider => routeProvider.Priority);

            foreach (var routeProvider in instances)
                routeProvider.RegisterRoutes(routeBuilder);
        }
        #endregion
    }
}
