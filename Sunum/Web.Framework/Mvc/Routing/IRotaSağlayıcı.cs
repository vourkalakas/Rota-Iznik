using Microsoft.AspNetCore.Routing;
namespace Web.Framework.Mvc.Routing
{
    public interface IRotaSağlayıcı
    {
        void RegisterRoutes(IRouteBuilder routeBuilder);
        int Priority { get; }
    }
}
