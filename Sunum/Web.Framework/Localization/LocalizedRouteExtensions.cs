using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Framework.Localization
{
    public static class LocalizedRouteExtensions
    {
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder, string name, string template)
        {
            return MapLocalizedRoute(routeBuilder, name, template, defaults: null);
        }
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder, string name, string template, object defaults)
        {
            return MapLocalizedRoute(routeBuilder, name, template, defaults, constraints: null);
        }
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder,
            string name, string template, object defaults, object constraints)
        {
            return MapLocalizedRoute(routeBuilder, name, template, defaults, constraints, dataTokens: null);
        }
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder,
            string name, string template, object defaults, object constraints, object dataTokens)
        {
            if (routeBuilder.DefaultHandler == null)
                throw new ArgumentNullException(nameof(routeBuilder));

            var inlineConstraintResolver = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

            routeBuilder.Routes.Add(new LocalizedRoute(routeBuilder.DefaultHandler, name, template,
                new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(dataTokens),
                inlineConstraintResolver));

            return routeBuilder;
        }
        public static void ClearSeoFriendlyUrlsCachedValueForRoutes(this IEnumerable<IRouter> routers)
        {
            if (routers == null)
                throw new ArgumentNullException(nameof(routers));

            foreach (var router in routers)
            {
                var routeCollection = router as RouteCollection;
                if (routeCollection == null)
                    continue;

                for (var i = 0; i < routeCollection.Count; i++)
                {
                    var route = routeCollection[i];
                    (route as LocalizedRoute)?.ClearSeoFriendlyUrlsCachedValue();
                }
            }
        }
    }
}