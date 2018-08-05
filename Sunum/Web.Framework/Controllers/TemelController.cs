using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using Core;
using Core.Domain.Kullanıcılar;
using Core.Altyapı;
using Services.Genel;
//using Services.Localization;
using Services.Logging;
using Services.Siteler;
using Web.Framework.Kendoui;
//using Web.Framework.Localization;
using Web.Framework.Mvc.Filtreler;
using Web.Framework.UI;

namespace Web.Framework.Controllers
{
    [SiteIpAdresi]
    [Sonİşlem]
    [SonZiyaretEdilenSayfa]
    [ParolaDoğrula]
    public abstract class TemelController : Controller
    {
        #region Rendering
        protected virtual string RenderViewComponentToString(string componentName, object arguments = null)
        {
            if (string.IsNullOrEmpty(componentName))
                throw new ArgumentNullException(nameof(componentName));

            var actionContextAccessor = HttpContext.RequestServices.GetService(typeof(IActionContextAccessor)) as IActionContextAccessor;
            if (actionContextAccessor == null)
                throw new Exception("IActionContextAccessor cannot be resolved");

            var context = actionContextAccessor.ActionContext;

            var viewComponentResult = ViewComponent(componentName, arguments);

            var viewData = this.ViewData;
            if (viewData == null)
            {
                throw new NotImplementedException();
            }

            var tempData = this.TempData;
            if (tempData == null)
            {
                throw new NotImplementedException();
            }

            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(
                    context,
                    NullView.Instance,
                    viewData,
                    tempData,
                    writer,
                    new HtmlHelperOptions());

                var viewComponentHelper = context.HttpContext.RequestServices.GetRequiredService<IViewComponentHelper>();
                (viewComponentHelper as IViewContextAware)?.Contextualize(viewContext);

                var result = viewComponentResult.ViewComponentType == null ?
                    viewComponentHelper.InvokeAsync(viewComponentResult.ViewComponentName, viewComponentResult.Arguments) :
                    viewComponentHelper.InvokeAsync(viewComponentResult.ViewComponentType, viewComponentResult.Arguments);

                result.Result.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }
        protected virtual string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }
        protected virtual string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }
        protected virtual string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }
        protected virtual string RenderPartialViewToString(string viewName, object model)
        {
            var razorViewEngine = EngineContext.Current.Resolve<IRazorViewEngine>();
            var actionContext = new ActionContext(this.HttpContext, this.RouteData, this.ControllerContext.ActionDescriptor, this.ModelState);
            
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;
            var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
            if (viewResult.View == null)
            {
                viewResult = razorViewEngine.GetView(null, viewName, false);
                if (viewResult.View == null)
                    throw new ArgumentNullException($"{viewName} view was not found");
            }
            using (var stringWriter = new StringWriter())
            {
                var viewContext = new ViewContext(actionContext, viewResult.View, ViewData, TempData, stringWriter, new HtmlHelperOptions());

                var t = viewResult.View.RenderAsync(viewContext);
                t.Wait();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        #endregion

        #region Notifications
        protected virtual void BaşarılıBildirimi(string message, bool persistForTheNextRequest = true)
        {
            BildirimEkle(BildirimTipi.Başarılı, message, persistForTheNextRequest);
        }
        protected virtual void UyarıBildirimi(string message, bool persistForTheNextRequest = true)
        {
            BildirimEkle(BildirimTipi.Uyarı, message, persistForTheNextRequest);
        }
        protected virtual void HataBildirimi(string message, bool persistForTheNextRequest = true)
        {
            BildirimEkle(BildirimTipi.Hata, message, persistForTheNextRequest);
        }
        protected virtual void HataBildirimi(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
        {
            if (logException)
                LogException(exception);

            BildirimEkle(BildirimTipi.Hata, exception.Message, persistForTheNextRequest);
        }
        protected void LogException(Exception exception)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logger = EngineContext.Current.Resolve<ILogger>();

            var customer = workContext.MevcutKullanıcı;
            logger.Hata(exception.Message, exception, customer);
        }
        protected virtual void BildirimEkle(BildirimTipi type, string message, bool persistForTheNextRequest)
        {
            var dataKey = $"rota.bildirimler.{type}";

            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }
        protected JsonResult KendoGridJsonHatası(string errorMessage)
        {
            var gridModel = new DataSourceSonucu
            {
                Hatalar = errorMessage
            };

            return Json(gridModel);
        }
        protected virtual void DüzenleLinkiniGörüntüle(string editPageUrl)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();

            pageHeadBuilder.DüzenleSayfaURLsiEkle(editPageUrl);
        }
        protected virtual int AktifSiteKapsamYapılandırmaAl(ISiteServisi siteService, IWorkContext workContext)
        {
            if (siteService.TümSiteler().Count < 2)
                return 0;

            var storeId = workContext.MevcutKullanıcı.ÖznitelikAl<int>(SistemKullanıcıÖznitelikAdları.YöneticiAlanıSiteKapsamAyarı);
            var store = siteService.SiteAlId(storeId);

            return store != null ? store.Id : 0;
        }

        #endregion

        #region Localization
        /*
        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService,
            IList<TLocalizedModelLocal> locales) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            AddLocales(languageService, locales, null);
        }

        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService,
            IList<TLocalizedModelLocal> locales, Action<TLocalizedModelLocal, int> configure) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            foreach (var language in languageService.GetAllLanguages(true))
            {
                var locale = Activator.CreateInstance<TLocalizedModelLocal>();
                locale.LanguageId = language.Id;

                if (configure != null)
                    configure.Invoke(locale, locale.LanguageId);

                locales.Add(locale);
            }
        }


        #endregion

        #region Security
        protected virtual IActionResult AccessDeniedView()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            //return Challenge();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = webHelper.GetRawUrl(this.Request) });
        }
        protected JsonResult AccessDeniedKendoGridJson()
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            return ErrorForKendoGridJson(localizationService.GetResource("Admin.AccessDenied.Description"));
        }
        */
        #endregion

        #region Security
        protected virtual IActionResult ErişimEngellendiGörünümü()
        {
            var webHelper = EngineContext.Current.Resolve<IWebYardımcısı>();

            //return Challenge();
            return RedirectToAction("ErişimEngellendi", "Güvenlik", new { pageUrl = webHelper.HamUrlAl(this.Request) });
        }
        protected JsonResult KendoGridJsonErişimEngellendi()
        {
            //var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            return KendoGridJsonHatası("Erişim Engellendi");
        }

        #endregion
    }
}
