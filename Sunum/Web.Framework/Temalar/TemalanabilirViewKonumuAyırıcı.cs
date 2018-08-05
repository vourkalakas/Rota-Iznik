using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Web.Framework.Temalar
{
    public class TemalanabilirViewKonumuAyırıcı : IViewLocationExpander
    {
        private const string THEME_KEY = "rota.temaadı";
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.AreaName?.Equals(AreaAdları.Admin) ?? false)
                return;

            var themeContext = (ITemaContext)context.ActionContext.HttpContext.RequestServices.GetService(typeof(ITemaContext));
            context.Values[THEME_KEY] = themeContext.MevcutTemaAdı;
        }
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue(THEME_KEY, out string theme))
            {
                viewLocations = new[] {
                        $"/Temalar/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Temalar/{theme}/Views/Shared/{{0}}.cshtml",
                    }
                    .Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
