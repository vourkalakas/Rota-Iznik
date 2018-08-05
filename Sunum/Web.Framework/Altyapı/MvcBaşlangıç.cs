using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Altyapı;
using Web.Framework.Altyapı.Uzantılar;

namespace Web.Framework.Altyapı
{
    public class MvcBaşlangıç : IStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddRotaMvc();
        }
        public void Configure(IApplicationBuilder application)
        {
            application.UseRotaMvc();
        }
        public int Order
        {
            get { return 1000; }
        }
    }
}
