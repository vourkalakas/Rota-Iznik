using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Altyapı;
using Web.Framework.Altyapı.Uzantılar;

namespace Web.Framework.Altyapı
{
    public class KimlikDoğrulamaBaşlangıç : IStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddRotaDataProtection();
            services.AddRotaAuthentication();
        }
        public void Configure(IApplicationBuilder application)
        {
            application.UseRotaAuthentication();
            application.UseCulture();
        }
        public int Order
        {
            get { return 500; }
        }
    }
}
