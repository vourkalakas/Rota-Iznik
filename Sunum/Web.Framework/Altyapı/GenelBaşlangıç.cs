using System.IO;
using System.Linq;
using ImageResizer.Plugins.PrettyGifs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Core;
using Core.Yapılandırma;
using Core.Data;
using Core.Domain.Güvenlik;
using Core.Altyapı;
using Web.Framework.Sıkıştırma;
using Web.Framework.Altyapı.Uzantılar;


namespace Web.Framework.Altyapı
{
    public class GenelBaşlangıç : IStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddResponseCompression();
            services.AddOptions();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddHttpSession();
            services.AddAntiForgery();
            services.AddLocalization();
            services.AddThemes();
            new PrettyGifs().Install(ImageResizer.Configuration.Config.Current);
        }
        public void Configure(IApplicationBuilder application)
        {
            var nopConfig = EngineContext.Current.Resolve<Config>();
            
            if (nopConfig.UseResponseCompression)
            {
                application.UseResponseCompression();
                application.UseMiddleware<ResponseCompressionVaryWorkaroundMiddleware>();
            }
            
            application.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(nopConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, nopConfig.StaticFilesCacheControl);
                }
            });
            //themes
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Temalar")),
                RequestPath = new PathString("/Temalar"),
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(nopConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, nopConfig.StaticFilesCacheControl);
                }
            });

            //plugins
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Eklentiler")),
                RequestPath = new PathString("/Eklentiler"),
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(nopConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl,
                            nopConfig.StaticFilesCacheControl);
                }
            };
            //whether database is installed
            if (DataAyarlarıYardımcısı.DatabaseYüklendi())
            {
                var securitySettings = EngineContext.Current.Resolve<GüvenlikAyarları>();
                if (!string.IsNullOrEmpty(securitySettings.EklentiUzantılarıKaraListesi))
                {
                    var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

                    foreach (var ext in securitySettings.EklentiUzantılarıKaraListesi
                        .Split(';', ',')
                        .Select(e => e.Trim().ToLower())
                        .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
                        .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
                    {
                        fileExtensionContentTypeProvider.Mappings.Remove(ext);
                    }

                    staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
                }
            }
            application.UseStaticFiles(staticFileOptions);
            
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".bak"] = MimeTipleri.ApplicationOctetStream;
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "db_backups")),
                RequestPath = new PathString("/db_backups"),
                ContentTypeProvider = provider
            });
            
            application.UseKeepAlive();
            application.UseInstallUrl();
            application.UseSession();
            application.UseRequestLocalization();
        }
        public int Order
        {
            get { return 100; }
        }
    }
}
