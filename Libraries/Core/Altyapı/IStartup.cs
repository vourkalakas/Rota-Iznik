using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Altyapı
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration);
        void Configure(IApplicationBuilder application);
        int Order { get; }
    }
}
