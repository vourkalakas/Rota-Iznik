using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Core.Yapılandırma;
using Core.Altyapı.BağımlılıkYönetimi;
using Core.Altyapı.Mapper;
using Core.Eklentiler;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

namespace Core.Altyapı
{
    public class Engine : IEngine
    {
        #region Utilities
        protected IServiceProvider GetServiceProvider()
        {
            var accessor = ServiceProvider.GetService<IHttpContextAccessor>();
            var context = accessor.HttpContext;
            return context != null ? context.RequestServices : ServiceProvider;
        }
        protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        {
            var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

            var instances = startupTasks
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            //execute tasks
            foreach (var task in instances)
                task.Execute();
        }

        protected virtual IServiceProvider RegisterDependencies(Config config, IServiceCollection services, ITypeFinder typeFinder)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();
            var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();

            var instances = dependencyRegistrars
                //.Where(dependencyRegistrar => PluginManager.FindPlugin(dependencyRegistrar).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            foreach (var dependencyRegistrar in instances)
                dependencyRegistrar.Register(containerBuilder, typeFinder, config);

            containerBuilder.Populate(services);
            _serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
            return _serviceProvider;
        }
        protected virtual void AddAutoMapper(IServiceCollection services, ITypeFinder typeFinder)
        {
            var mapperConfigurations = typeFinder.FindClassesOfType<IMapperAyarları>();

            var instances = mapperConfigurations
                .Where(mapperConfiguration => EklentiYönetici.EklentiBul(mapperConfiguration)?.Kuruldu ?? true) //ignore not installed plugins
                .Select(mapperConfiguration => (IMapperAyarları)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            var config = new MapperConfiguration(cfg => {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });
            services.AddAutoMapper();
            AutoMapperAyarları.Init(config);
        }

        #endregion

        #region Methods
        public void Initialize(IServiceCollection services)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
            var provider = services.BuildServiceProvider();
            var hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();
            var nopConfig = provider.GetRequiredService<Config>();
            GenelYardımcı.KökKlasör = hostingEnvironment.ContentRootPath;

            //eklentileri yükle
            var mvcCoreBuilder = services.AddMvcCore();
            EklentiYönetici.Initialize(mvcCoreBuilder.PartManager, nopConfig);
        }
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;
            
            var tf = Resolve<ITypeFinder>();
            assembly = tf.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            return assembly;
        }
        public IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            var typeFinder = new WebAppTypeFinder();
            var startupConfigurations = typeFinder.FindClassesOfType<IStartup>();
            
            var instances = startupConfigurations
                .Where(startup => EklentiYönetici.EklentiBul(startup)?.Kuruldu ?? true) //ignore not installed plugins
                .Select(startup => (IStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //configure services
            foreach (var instance in instances)
                instance.ConfigureServices(services, configuration);
            
            AddAutoMapper(services, typeFinder);
            
            var nopConfig = services.BuildServiceProvider().GetService<Config>();
            RegisterDependencies(nopConfig, services, typeFinder);
            
            if (!nopConfig.IgnoreStartupTasks)
                RunStartupTasks(typeFinder);
            
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.SetData("DataDirectory", GenelYardımcı.MapPath("~/App_Data/"));

            return _serviceProvider;
        }
        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            var typeFinder = Resolve<ITypeFinder>();
            var startupConfigurations = typeFinder.FindClassesOfType<IStartup>();
            
            var instances = startupConfigurations
                .Where(startup => EklentiYönetici.EklentiBul(startup)?.Kuruldu ?? true) //ignore not installed plugins
                .Select(startup => (IStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //configure request pipeline
            foreach (var instance in instances)
                instance.Configure(application);
        }

        public T Resolve<T>() where T : class
        {
            return (T)GetServiceProvider().GetRequiredService(typeof(T));
        }
        public virtual object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new Hata("Unknown dependency");
                        return service;
                    });
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }
            throw new Hata("No constructor was found that had all the dependencies satisfied.", innerException);
        }
        public object Resolve(Type type)
        {
            return GetServiceProvider().GetRequiredService(type);
        }
        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        #endregion

        #region Properties
        private IServiceProvider _serviceProvider { get; set; }
        public virtual IServiceProvider ServiceProvider => _serviceProvider;

        #endregion
    }
}
