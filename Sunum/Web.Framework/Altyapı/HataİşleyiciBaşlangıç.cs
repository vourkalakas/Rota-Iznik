using Core.Altyapı;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Framework.Altyapı.Uzantılar;

namespace Web.Framework.Altyapı
{
    public class HataİşleyiciBaşlangıç : IStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
        }
        
        public void Configure(IApplicationBuilder application)
        {
            application.UseRotaExceptionHandler();
            application.UseBadRequestResult();
            application.UsePageNotFound();
        }
        public int Order
        {
            get { return 0; }
        }
    }
}
