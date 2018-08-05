using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Core;
using Core.Önbellek;
using Core.Yapılandırma;
using Core.Data;
using Core.Altyapı;
using Core.Altyapı.BağımlılıkYönetimi;
using Core.Eklentiler;
using Data;
//using Services.Affiliates;
using Services.KimlikDoğrulama;
using Services.KimlikDoğrulama.Harici;
using Services.Blog;
using Services.Katalog;
using Services.Cms;
using Services.Genel;
using Services.Yapılandırma;
using Services.Kullanıcılar;
using Services.Klasör;
//using Services.Discounts;
using Services.Olaylar;
//using Services.ExportImport;
using Services.Forumlar;
using Services.Yardımcılar;
//using Services.Installation;
//using Services.Localization;
using Services.Logging;
using Services.Medya;
using Services.Mesajlar;
using Services.Haberler;
//using Services.Orders;
//using Services.Payments;
//using Services.Eklentiler;
using Services.Anketler;
using Services.Güvenlik;
using Services.Seo;
//using Services.Shipping;
//using Services.Shipping.Date;
using Services.Siteler;
//using Services.Tax;
using Services.Temalar;
using Services.Sayfalar;
//using Services.Vendors;
using Web.Framework.Mvc.Routing;
using Web.Framework.Temalar;
//using Web.Framework.UI;
using Services.EkTanımlamalar;
using Services.Konum;
using Services.Tanımlamalar;
using Services.Teklifler;
using Services.Yetkililer;
using Services.Görüşmeler;
using Services.Finans;
using Services.Hint;
using Services.Kongre;
using Services.Notlar;
using Services.DovizServisi;
using Services.Localization;
using Web.Framework.UI;

namespace Web.Framework.Altyapı
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, Config config)
        {
            builder.RegisterType<WebYardımcısı>().As<IWebYardımcısı>().InstancePerLifetimeScope();
            builder.RegisterType<KullanıcıAracıYardımcısı>().As<IKullanıcıAracıYardımcısı>().InstancePerLifetimeScope();

            //data layer
            var dataSettingsManager = new DataAyarlarıYönetici();
            var dataProviderSettings = dataSettingsManager.AyarlarıYükle();
            builder.Register(c => dataSettingsManager.AyarlarıYükle()).As<DataAyarları>();
            builder.Register(x => new EfDataSağlayıcıYöneticisi(x.Resolve<DataAyarları>())).As<TemelVeriSağlayıcıYöneticisi>().InstancePerDependency();

            builder.Register(x => x.Resolve<TemelVeriSağlayıcıYöneticisi>().DataSağlayıcıYükle()).As<IDataSağlayıcı>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.Geçerli())
            {
                var efDataProviderManager = new EfDataSağlayıcıYöneticisi(dataSettingsManager.AyarlarıYükle());
                var dataProvider = efDataProviderManager.DataSağlayıcıYükle();
                dataProvider.BağlantıFabrikasıBaşlat();

                builder.Register<IDbContext>(c => new ObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            }
            else
                builder.Register<IDbContext>(c => new ObjectContext(dataSettingsManager.AyarlarıYükle().DataConnectionString)).InstancePerLifetimeScope();

            //repositories
            builder.RegisterGeneric(typeof(EfDepo<>)).As(typeof(IDepo<>)).InstancePerLifetimeScope();

            //plugins
            builder.RegisterType<EklentiBulucu>().As<IEklentiBulucu>().InstancePerLifetimeScope();

            //cache manager
            builder.RegisterType<İstekBaşınaÖnbellekYöneticisi>().As<IÖnbellekYönetici>().InstancePerLifetimeScope();

            //static cache manager
            if (config.RedisCachingEnabled)
            {
                builder.RegisterType<RedisConnectionWrapper>().As<IRedisConnectionWrapper>().SingleInstance();
                builder.RegisterType<RedisÖnbellekYönetici>().As<IStatikÖnbellekYönetici>().InstancePerLifetimeScope();
            }
            else
                builder.RegisterType<ÖnbellekYöneticiHafıza>().As<IStatikÖnbellekYönetici>().SingleInstance();

            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            //store context
            builder.RegisterType<WebSiteContext>().As<ISiteContext>().InstancePerLifetimeScope();

            //services
            //builder.RegisterType<BackInStockSubscriptionService>().As<IBackInStockSubscriptionService>().InstancePerLifetimeScope();
            builder.RegisterType<KategoriServisi>().As<IKategoriServisi>().InstancePerLifetimeScope();
            //builder.RegisterType<CompareProductsService>().As<ICompareProductsService>().InstancePerLifetimeScope();
            //builder.RegisterType<RecentlyViewedProductsService>().As<IRecentlyViewedProductsService>().InstancePerLifetimeScope();
            //builder.RegisterType<ManufacturerService>().As<IManufacturerService>().InstancePerLifetimeScope();
            //builder.RegisterType<PriceFormatter>().As<IPriceFormatter>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductAttributeFormatter>().As<IProductAttributeFormatter>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductAttributeParser>().As<IProductAttributeParser>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductAttributeService>().As<IProductAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<OlayYayınlayıcı>().As<IOlayYayınlayıcı>().SingleInstance();
            builder.RegisterType<AbonelikServisi>().As<IAbonelikServisi>().SingleInstance();
            builder.RegisterType<İzinServisi>().As<IİzinServisi>()
                .WithParameter(ResolvedParameter.ForNamed<IÖnbellekYönetici>("cache_static"))
                .InstancePerLifetimeScope();
            builder.RegisterType<KategoriServisi>().As<IKategoriServisi>().InstancePerLifetimeScope();
            builder.RegisterType<SayfalarServisi>().As<ISayfalarServisi>().InstancePerLifetimeScope();
            builder.RegisterType<BankalarServisi>().As<IBankalarServisi>().InstancePerLifetimeScope();
            builder.RegisterType<MusteriSektorServisi>().As<IMusteriSektorServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TedarikciSektorServisi>().As<ITedarikciSektorServisi>().InstancePerLifetimeScope();
            builder.RegisterType<HariciSektorServisi>().As<IHariciSektorServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TeklifKalemiServisi>().As<ITeklifKalemiServisi>().InstancePerLifetimeScope();
            builder.RegisterType<UnvanlarServisi>().As<IUnvanlarServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KonumServisi>().As<IKonumServisi>().InstancePerLifetimeScope();
            builder.RegisterType<MusteriServisi>().As<IMusteriServisi>().InstancePerLifetimeScope();
            builder.RegisterType<OtelServisi>().As<IOtelServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TedarikciServisi>().As<ITedarikciServisi>().InstancePerLifetimeScope();
            builder.RegisterType<YDAcenteServisi>().As<IYDAcenteServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TeklifServisi>().As<ITeklifServisi>().InstancePerLifetimeScope();
            builder.RegisterType<BagliTeklifOgesiServisi>().As<IBagliTeklifOgesiServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TeklifHariciServisi>().As<ITeklifHariciServisi>().InstancePerLifetimeScope();
            builder.RegisterType<BagliTeklifOgesiHariciServisi>().As<IBagliTeklifOgesiHariciServisi>().InstancePerLifetimeScope();
            builder.RegisterType<YetkiliServisi>().As<IYetkiliServisi>().InstancePerLifetimeScope();
            builder.RegisterType<GorusmeRaporlariServisi>().As<IGorusmeRaporlariServisi>().InstancePerLifetimeScope();
            builder.RegisterType<OdemeFormuServisi>().As<IOdemeFormuServisi>().InstancePerLifetimeScope();
            builder.RegisterType<HintServisi>().As<IHintServisi>().InstancePerLifetimeScope();
            builder.RegisterType<PdfServisi>().As<IPdfServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KatilimciServisi>().As<IKatilimciServisi>().InstancePerLifetimeScope();
            builder.RegisterType<RefakatciServisi>().As<IRefakatciServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KayitServisi>().As<IKayitServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KonaklamaServisi>().As<IKonaklamaServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KursServisi>().As<IKursServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TransferServisi>().As<ITransferServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KongreServisi>().As<IKongreServisi>().InstancePerLifetimeScope();
            builder.RegisterType<NotServisi>().As<INotServisi>().InstancePerLifetimeScope();
            builder.RegisterType<DovizServisi>().As<IDovizServisi>().InstancePerLifetimeScope();
            builder.RegisterType<MesajServisi>().As<IMesajServisi>().InstancePerLifetimeScope();
            builder.RegisterType<MesajlarServisi>().As<IMesajlarServisi>().InstancePerLifetimeScope();
            //builder.RegisterType<TestServisi>().As<ITestServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KontenjanServisi>().As<IKontenjanServisi>().InstancePerLifetimeScope();
            builder.RegisterType<RotaYayınlayıcı>().As<IRotaYayınlayıcı>().SingleInstance();
            builder.RegisterType<WidgetServisi>().As<IWidgetServisi>().InstancePerLifetimeScope();

            builder.RegisterType<BültenAbonelikServisi>().As<IBültenAbonelikServisi>().InstancePerLifetimeScope();
            builder.RegisterType<ÜlkeServisi>().As<IÜlkeServisi>().InstancePerLifetimeScope();
            //builder.RegisterType<AçıkYetkilendirmeServisi>().As<IAçıkYetkilendirmeServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TarihYardımcısı>().As<ITarihYardımcısı>().InstancePerLifetimeScope();
            builder.RegisterType<KullanıcıİşlemServisi>().As<IKullanıcıİşlemServisi>()
                .WithParameter(ResolvedParameter.ForNamed<IÖnbellekYönetici>("cache_static"))
                .InstancePerLifetimeScope();
            builder.RegisterType<MesajTemasıServisi>().As<IMesajTemasıServisi>().InstancePerLifetimeScope();
            builder.RegisterType<MesajServisi>().As<IMesajServisi>().InstancePerLifetimeScope();
            builder.RegisterType<EmailHesapServisi>().As<IEmailHesapServisi>().InstancePerLifetimeScope();
            builder.RegisterType<BekleyenMailServisi>().As<IBekleyenMailServisi>().InstancePerLifetimeScope();
            builder.RegisterType<ForumServisi>().As<IForumServisi>().InstancePerLifetimeScope();
            builder.RegisterType<UrlKayıtServisi>().As<IUrlKayıtServisi>()
               .WithParameter(ResolvedParameter.ForNamed<IÖnbellekYönetici>("cache_static"))
               .InstancePerLifetimeScope();
            builder.RegisterType<AclServisi>().As<IAclServisi>()
                .WithParameter(ResolvedParameter.ForNamed<IÖnbellekYönetici>("cache_static"))
                .InstancePerLifetimeScope();
            builder.RegisterType<SiteMappingServisi>().As<ISiteMappingServisi>()
                .WithParameter(ResolvedParameter.ForNamed<IÖnbellekYönetici>("cache_static"))
                .InstancePerLifetimeScope();
            builder.RegisterType<SayfaTemaServisi>().As<ISayfaTemaServisi>().InstancePerLifetimeScope();
            builder.RegisterType<HaberServisi>().As<IHaberServisi>().InstancePerLifetimeScope();
            builder.RegisterType<BlogServisi>().As<IBlogServisi>().InstancePerLifetimeScope();
            builder.RegisterType<AnketServisi>().As<IAnketServisi>().InstancePerLifetimeScope();
            builder.RegisterType<TamMetinServisi>().As<ITamMetinServisi>().InstancePerLifetimeScope();
            builder.RegisterType<EmailGönderici>().As<IEmailGönderici>().InstancePerLifetimeScope();
            builder.RegisterType<DownloadServisi>().As<IDownloadServisi>().InstancePerLifetimeScope();
            builder.RegisterType<XlsDosyaServisi>().As<IXlsDosyaServisi>().InstancePerLifetimeScope();
            //builder.RegisterType<XlsServisi>().As<IXlsServisi>().InstancePerLifetimeScope();
            builder.RegisterType<XlsUploadServisi>().As<IXlsUploadServisi>().InstancePerLifetimeScope();
            builder.RegisterType<VarsayılanLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<AyarlarServisi>().As<IAyarlarServisi>().InstancePerLifetimeScope();
            builder.RegisterType<SiteServisi>().As<ISiteServisi>().InstancePerLifetimeScope();
            builder.RegisterType<KullanıcıServisi>().As<IKullanıcıServisi>().InstancePerLifetimeScope();
            builder.RegisterType<GenelÖznitelikServisi>().As<IGenelÖznitelikServisi>().InstancePerLifetimeScope();
            builder.RegisterType<ÇerezKimlikDoğrulamaServisi>().As<IKimlikDoğrulamaServisi>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerLifetimeScope();
            builder.RegisterType<SayfaHeadOluşturucu>().As<ISayfaHeadOluşturucu>().InstancePerLifetimeScope();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>().InstancePerLifetimeScope();
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            //store context
            builder.RegisterType<WebSiteContext>().As<ISiteContext>().InstancePerLifetimeScope();

            builder.RegisterType<TemaContext>().As<ITemaContext>().InstancePerLifetimeScope();
            builder.RegisterType<TemaSağlayıcı>().As<ITemaSağlayıcı>().InstancePerLifetimeScope();

            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().InstancePerLifetimeScope();

            //register all settings
            builder.RegisterSource(new SettingsSource());

            //picture service
            if (!string.IsNullOrEmpty(config.AzureBlobStorageConnectionString))
                builder.RegisterType<AzurePictureService>().As<IResimServisi>().InstancePerLifetimeScope();
            else
                builder.RegisterType<ResimServisi>().As<IResimServisi>().InstancePerLifetimeScope();
            /*
            //installation service
            if (!DataAyarlarıYardımcısı.DatabaseYüklendi())
            {
                if (config.UseFastInstallationService)
                    builder.RegisterType<SqlFileInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();
                else
                    builder.RegisterType<CodeFirstInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();
            }
            
            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }
            */
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }


    /// <summary>
    /// Setting source
    /// </summary>
    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// Registrations for
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="registrations">Registrations</param>
        /// <returns>Registrations</returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(IAyarlar).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : IAyarlar, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    var currentStoreId = 6;
                    //uncomment the code below if you want load settings per store only when you have two stores installed.
                    //var currentStoreId = c.Resolve<IStoreService>().GetAllStores().Count > 1
                    //    c.Resolve<IStoreContext>().CurrentStore.Id : 0;

                    //although it's better to connect to your database and execute the following SQL:
                    //DELETE FROM [Setting] WHERE [StoreId] > 0
                    return c.Resolve<IAyarlarServisi>().AyarYükle<TSettings>(currentStoreId);
                })
                .InstancePerLifetimeScope()
                .CreateRegistration();
        }
        public bool IsAdapterForIndividualComponents { get { return false; } }
    }
}
