using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Core;
using Core.Yapılandırma;
using Core.Data;
using Core.Domain;
using Core.Http;
using Core.Altyapı;
using Services.KimlikDoğrulama;
using Services.Logging;
using Services.Güvenlik;
using System.Threading.Tasks;
using Web.Framework.Globalization;
using Web.Framework.Mvc.Routing;

namespace Web.Framework.Altyapı.Uzantılar
{
    public static class ApplicationBuilderUzantıları
    {
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }
        public static void UseRotaExceptionHandler(this IApplicationBuilder application)
        {
            var nopConfig = EngineContext.Current.Resolve<Config>();
            var hostingEnvironment = EngineContext.Current.Resolve<IHostingEnvironment>();
            var useDetailedExceptionPage = nopConfig.DisplayFullErrorStack || hostingEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                application.UseDeveloperExceptionPage();
            }
            else
            {
                application.UseExceptionHandler("/errorpage.htm");
            }

            application.UseExceptionHandler(handler =>
            {
                handler.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return Task.CompletedTask;

                    try
                    {
                        if (DataAyarlarıYardımcısı.DatabaseYüklendi())
                        {
                            //get current customer
                            var currentCustomer = EngineContext.Current.Resolve<IWorkContext>().MevcutKullanıcı;

                            //log error
                            EngineContext.Current.Resolve<ILogger>().Hata(exception.Message, exception, currentCustomer);
                        }
                    }
                    finally
                    {
                        throw exception;
                    }
                });
            });
        }
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = EngineContext.Current.Resolve<IWebYardımcısı>();
                    if (!webHelper.SabitKaynak())
                    {
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;

                        context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature()
                        {
                            OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                            OriginalPath = originalPath.Value,
                            OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null,
                        });

                        context.HttpContext.Request.Path = "/sayfa-bulunamadı";
                        context.HttpContext.Request.QueryString = QueryString.Empty;

                        try
                        {
                            await context.Next(context.HttpContext);
                        }
                        finally
                        {
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                            context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
                        }
                    }
                }
            });
        }
        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    logger.Hata("Error 400. Bad request", null, kullanıcı: workContext.MevcutKullanıcı);
                }

                return Task.CompletedTask;
            });
        }
        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            application.UseMiddleware<KeepAliveMiddleware>();
        }
        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            application.UseMiddleware<InstallUrlMiddleware>();
        }
        public static void UseRotaAuthentication(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
                return;

            application.UseMiddleware<AuthenticationMiddleware>();
        }
        public static void UseCulture(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
                return;

            application.UseMiddleware<CultureMiddleware>();
        }
        public static void UseRotaMvc(this IApplicationBuilder application)
        {
            application.UseMvc(routeBuilder =>
            {
                EngineContext.Current.Resolve<IRotaYayınlayıcı>().RegisterRoutes(routeBuilder);
            });
        }
        
    }
}
