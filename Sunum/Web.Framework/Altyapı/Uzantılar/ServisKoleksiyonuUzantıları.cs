using System;
using System.IO;
using System.Linq;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Core;
using Core.Önbellek;
using Core.Yapılandırma;
using Core.Data;
using Core.Altyapı;
using Core.Eklentiler;
using Services.KimlikDoğrulama;
using Services.KimlikDoğrulama.Harici;
using Services.Logging;
using Web.Framework.FluentValidation;
using Web.Framework.Mvc.ModelBağlama;
using Web.Framework.Temalar;

namespace Web.Framework.Altyapı.Uzantılar
{
    public static class ServisKoleksiyonuUzantıları
    {
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.ConfigureStartupConfig<Config>(configuration.GetSection("Rota"));
            services.ConfigureStartupConfig<HostingAyarları>(configuration.GetSection("Hosting"));
            services.AddHttpContextAccessor();
            var engine = EngineContext.Create();
            engine.Initialize(services);
            var serviceProvider = engine.ConfigureServices(services, configuration);

            if (DataAyarlarıYardımcısı.DatabaseYüklendi())
            {
                EngineContext.Current.Resolve<ILogger>().Bilgi("Uygulama başladı", null, null);
            }

            return serviceProvider;
        }
        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var config = new TConfig();
            configuration.Bind(config);
            services.AddSingleton(config);

            return config;
        }
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
        public static void AddAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = ".Rota.Antiforgery";
            });
        }
        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Rota.Session";
                options.Cookie.HttpOnly = true;
            });
        }
        public static void AddThemes(this IServiceCollection services)
        {
            if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
                return;

            //themes support
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TemalanabilirViewKonumuAyırıcı());
            });
        }
        public static void AddRotaDataProtection(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetRequiredService<Config>();
            if (config.RedisCachingEnabled && config.PersistDataProtectionKeysToRedis)
            {
                services.AddDataProtection().PersistKeysToRedis(
                    () =>
                    {
                        var redisConnectionWrapper = EngineContext.Current.Resolve<IRedisConnectionWrapper>();
                        return redisConnectionWrapper.GetDatabase();
                    }, RedisYapılandırması.DataProtectionKeysName);
            }
            else
            {
                var dataProtectionKeysPath = GenelYardımcı.MapPath("~/App_Data/DataProtectionKeys");
                var dataProtectionKeysFolder = new DirectoryInfo(dataProtectionKeysPath);

                services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
            }
        }
        public static void AddRotaAuthentication(this IServiceCollection services)
        {
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = ÇerezYetkilendirmeVarsayılanları.AuthenticationScheme;
                options.DefaultSignInScheme = ÇerezYetkilendirmeVarsayılanları.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(ÇerezYetkilendirmeVarsayılanları.AuthenticationScheme, options =>
            {
                options.Cookie.Name = ÇerezYetkilendirmeVarsayılanları.CookiePrefix + ÇerezYetkilendirmeVarsayılanları.AuthenticationScheme;
                options.Cookie.HttpOnly = true;
                options.LoginPath = ÇerezYetkilendirmeVarsayılanları.LoginPath;
                options.AccessDeniedPath = ÇerezYetkilendirmeVarsayılanları.AccessDeniedPath;
            });

            //add external authentication
            authenticationBuilder.AddCookie(ÇerezYetkilendirmeVarsayılanları.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = ÇerezYetkilendirmeVarsayılanları.CookiePrefix + ÇerezYetkilendirmeVarsayılanları.ExternalAuthenticationScheme;
                options.Cookie.HttpOnly = true;
                options.LoginPath = ÇerezYetkilendirmeVarsayılanları.LoginPath;
                options.AccessDeniedPath = ÇerezYetkilendirmeVarsayılanları.AccessDeniedPath;
            });

            //register and configure external authentication plugins now
            var typeFinder = new WebAppTypeFinder();
            var externalAuthConfigurations = typeFinder.FindClassesOfType<IHariciYetkilendirmeRegistrar>();
            var externalAuthInstances = externalAuthConfigurations
                .Where(x => EklentiYönetici.EklentiBul(x)?.Kuruldu ?? true) //ignore not installed plugins
                .Select(x => (IHariciYetkilendirmeRegistrar)Activator.CreateInstance(x));

            foreach (var instance in externalAuthInstances)
                instance.Yapılandır(authenticationBuilder);
        }
        public static IMvcBuilder AddRotaMvc(this IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddSessionStateTempDataProvider();
            mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new MetaDataSağlayıcı()));
            mvcBuilder.AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new ModelBağlamaSağlayıcı()));
            mvcBuilder.AddFluentValidation(configuration => configuration.ValidatorFactoryType = typeof(DoğrulayıcıFabrikası));
            return mvcBuilder;
        }
    }
}
