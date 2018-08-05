using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

namespace Core.Altyapı
{
    public interface IEngine
    {
        void Initialize(IServiceCollection services);
        IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot configuration);
        void ConfigureRequestPipeline(IApplicationBuilder application);
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        IEnumerable<T> ResolveAll<T>();
        object ResolveUnregistered(Type type);
    }
}
