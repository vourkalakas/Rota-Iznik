using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Core;
using Core.Önbellek;
using Core.Domain;
using Core.Domain.Blogs;
using Core.Domain.Katalog;
using Core.Domain.Genel;
using Core.Domain.Kullanıcılar;
using Core.Domain.Forum;
using Core.Domain.Localization;
using Core.Domain.Haber;
using Core.Domain.Siparişler;
using Services.Katalog;
using Services.Genel;
using Services.Kullanıcılar;
using Services.Klasör;
using Services.Forumlar;
using Services.Localization;
using Services.Medya;
using Services.Güvenlik;
using Services.Seo;
using Services.Temalar;
using Services.Sayfalar;
//using Web.Framework.Güvenlik.Captcha;
using Web.Framework.Temalar;
using Web.Framework.UI;
using Web.Altyapı.Önbellek;
//using Web.Models.Katalog;
using Web.Models.Genel;
//using Web.Models.Sayfalar;

namespace Web.Fabrika
{
    public partial class GenelModelFactory : IGenelModelFactory
    {
        #region Fields

        private readonly IKategoriServisi _categoryService;
        private readonly ISayfalarServisi _topicService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _storeContext;
        private readonly ITemaContext _themeContext;
        private readonly ITemaSağlayıcı _themeProvider;
        private readonly IForumServisi _forumservice;
        private readonly IGenelÖznitelikServisi _genericAttributeService;
        private readonly IWebYardımcısı _webHelper;
        private readonly IİzinServisi _permissionService;
        private readonly IStatikÖnbellekYönetici _cacheManager;
        private readonly ISayfaHeadOluşturucu _pageHeadBuilder;
        private readonly IResimServisi _pictureService;
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly KatalogAyarları _catalogSettings;
        private readonly SiteBilgiAyarları _storeInformationSettings;
        private readonly GenelAyarlar _commonSettings;
        private readonly BlogAyarları _blogSettings;
        private readonly ForumAyarları _forumSettings;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public GenelModelFactory(IKategoriServisi categoryService,
            ISayfalarServisi topicService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ISiteContext storeContext,
            ITemaContext themeContext,
            ITemaSağlayıcı themeProvider,
            IForumServisi forumservice,
            IGenelÖznitelikServisi genericAttributeService,
            IWebYardımcısı webHelper,
            IİzinServisi permissionService,
            IStatikÖnbellekYönetici cacheManager,
            ISayfaHeadOluşturucu pageHeadBuilder,
            IResimServisi pictureService,
            IHostingEnvironment hostingEnvironment,

            KatalogAyarları catalogSettings,
            SiteBilgiAyarları storeInformationSettings,
            GenelAyarlar commonSettings,
            BlogAyarları blogSettings,
            ForumAyarları forumSettings,
            LocalizationSettings localizationSettings)
        {
            this._categoryService = categoryService;
            this._topicService = topicService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._cacheManager = cacheManager;
            this._pageHeadBuilder = pageHeadBuilder;
            this._pictureService = pictureService;
            this._hostingEnvironment = hostingEnvironment;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._commonSettings = commonSettings;
            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities
        protected virtual int GetUnreadPrivateMessages()
        {
            var result = 0;
            var customer = _workContext.MevcutKullanıcı;
            if (_forumSettings.ÖzelMesajEtkin && !customer.IsGuest())
            {
                var privateMessages = _forumservice.TümÖzelMesajlarıAl(_storeContext.MevcutSite.Id,
                    0, customer.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }

        #endregion

        #region Methods
        /*
        public virtual LogoModel PrepareLogoModel()
        {
            var model = new LogoModel
            {
                StoreName = _storeContext.MevcutSite.GetLocalized(x => x.Adı)
            };

            var cacheKey = string.Format(ModelÖnbellekOlayTüketici.STORE_LOGO_PATH, _storeContext.MevcutSite.Id, _themeContext.MevcutTemaAdı, _webHelper.MevcutBağlantıGüvenli());
            model.LogoPath = _cacheManager.Al(cacheKey, () =>
            {
                var logo = "";
                var logoPictureId = _storeInformationSettings.LogoResimId;
                if (logoPictureId > 0)
                {
                    logo = _pictureService.ResimUrlAl(logoPictureId, varsayılanResimGöster: false);
                }
                if (string.IsNullOrEmpty(logo))
                {
                    //use default logo
                    logo = $"{_webHelper.SiteKonumuAl()}Temalar/{_themeContext.MevcutTemaAdı}/Content/images/logo.png";
                }
                return logo;
            });

            return model;
        }
        public virtual LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _cacheManager.Al(string.Format(ModelÖnbellekOlayTüketici.AVAILABLE_LANGUAGES_MODEL_KEY, _storeContext.MevcutSite.Id), () =>
            {
                var result = _languageService
                    .GetAllLanguages(storeId: _storeContext.MevcutSite.Id)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Adı,
                        FlagImageFileName = x.BayrakResmiDosyaAdı,
                    })
                    .ToList();
                return result;
            });

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = _workContext.MevcutDil.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            return model;
        }
        */
        public virtual HeaderLinksModel PrepareHeaderLinksModel()
        {
            var customer = _workContext.MevcutKullanıcı;

            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = string.Format(_localizationService.GetResource("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (_forumSettings.ÖzelMesajUyarısıGöster &&
                    !customer.ÖznitelikAl <bool>(SistemKullanıcıÖznitelikAdları.YeniÖzelMesajBilgisi, _storeContext.MevcutSite.Id))
                {
                    _genericAttributeService.ÖznitelikKaydet(customer, SistemKullanıcıÖznitelikAdları.YeniÖzelMesajBilgisi, true, _storeContext.MevcutSite.Id);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel
            {
                Yetkilendirildi = customer.IsRegistered(),
                KullanıcıAdı = customer.IsRegistered() ? customer.KullanıcıAdıFormatı() : "",
                SepetEtkin = _permissionService.YetkiVer(StandartİzinSağlayıcı.SepetEtkin),
                ÖzelMesajlarİzinli = customer.IsRegistered() && _forumSettings.ÖzelMesajEtkin,
                OkunmamışÖzelMesajlar = unreadMessage,
                MesajUyarısı = alertMessage,
            };
            //performance optimization (use "HasShoppingCartItems" property)

            return model;
        }
        
        public virtual AdminHeaderLinksModel PrepareAdminHeaderLinksModel()
        {
            var customer = _workContext.MevcutKullanıcı;

            var model = new AdminHeaderLinksModel
            {
                KimliğeBürünmüşKulllanıcıAdı = customer.IsRegistered() ? customer.KullanıcıAdıFormatı() : "",
                KullanıcıKimliğeBüründü = _workContext.OrijinalKullanıcıyıTaklitEt != null,
                AdminLinkGörüntüle = _permissionService.YetkiVer(StandartİzinSağlayıcı.YöneticiBölgesiErişimi),
                SayfayıDüzenle = _pageHeadBuilder.DüzenleSayfaURLsiAl()
            };
            return model;
        }
        /*
        public virtual SocialModel PrepareSocialModel()
        {
            var model = new SocialModel
            {
                FacebookLink = _storeInformationSettings.FacebookLink,
                TwitterLink = _storeInformationSettings.TwitterLink,
                YoutubeLink = _storeInformationSettings.YoutubeLink,
                GooglePlusLink = _storeInformationSettings.GooglePlusLink,
                MevcutDilId = _workContext.MevcutDil.Id,
                NewsEnabled = _newsSettings.Enabled,
            };

            return model;
        }
        public virtual FooterModel PrepareFooterModel()
        {
            //footer topics
            var topicCacheKey = string.Format(ModelÖnbellekOlayTüketici.TOPIC_FOOTER_MODEL_KEY,
                _workContext.MevcutDil.Id,
                _storeContext.MevcutSite.Id,
                string.Join(",", _workContext.MevcutKullanıcı.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(_storeContext.MevcutSite.Id)
                .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                .Select(t => new FooterModel.FooterTopicModel
                {
                    Id = t.Id,
                    Name = t.GetLocalized(x => x.Title),
                    SeName = t.GetSeName(),
                    IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                    IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                    IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                })
                .ToList()
            );

            //model
            var model = new FooterModel
            {
                StoreName = _storeContext.MevcutSite.GetLocalized(x => x.Name),
                WishlistEnabled = _permissionService.Authorize(StandartİzinSağlayıcı.EnableWishlist),
                ShoppingCartEnabled = _permissionService.Authorize(StandartİzinSağlayıcı.EnableShoppingCart),
                SitemapEnabled = _commonSettings.SitemapEnabled,
                MevcutDilId = _workContext.MevcutDil.Id,
                BlogEnabled = _blogSettings.Enabled,
                CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                NewsEnabled = _newsSettings.Enabled,
                RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                DisplayTaxShippingInfoFooter = _catalogSettings.DisplayTaxShippingInfoFooter,
                HidePoweredByNopCommerce = _storeInformationSettings.HidePoweredByNopCommerce,
                AllowCustomersToApplyForVendorAccount = _vendorSettings.AllowCustomersToApplyForVendorAccount,
                Topics = cachedTopicModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact us model</returns>
        public virtual ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties)
            {
                model.Email = _workContext.MevcutKullanıcı.Email;
                model.FullName = _workContext.MevcutKullanıcı.GetFullName();
            }
            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

            return model;
        }

        /// <summary>
        /// Prepare the contact vendor model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact vendor model</returns>
        public virtual ContactVendorModel PrepareContactVendorModel(ContactVendorModel model, Vendor vendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (!excludeProperties)
            {
                model.Email = _workContext.MevcutKullanıcı.Email;
                model.FullName = _workContext.MevcutKullanıcı.GetFullName();
            }

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            model.VendorId = vendor.Id;
            model.VendorName = vendor.GetLocalized(x => x.Name);

            return model;
        }

        /// <summary>
        /// Prepare the sitemap model
        /// </summary>
        /// <returns>Sitemap model</returns>
        public virtual SitemapModel PrepareSitemapModel()
        {
            var cacheKey = string.Format(ModelÖnbellekOlayTüketici.SITEMAP_PAGE_MODEL_KEY,
                _workContext.MevcutDil.Id,
                string.Join(",", _workContext.MevcutKullanıcı.GetCustomerRoleIds()),
                _storeContext.MevcutSite.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new SitemapModel
                {
                    BlogEnabled = _blogSettings.Enabled,
                    ForumEnabled = _forumSettings.ForumsEnabled,
                    NewsEnabled = _newsSettings.Enabled,
                };
                //categories
                if (_commonSettings.SitemapIncludeCategories)
                {
                    var categories = _categoryService.GetAllCategories(storeId: _storeContext.MevcutSite.Id);
                    model.Categories = categories.Select(category => new CategorySimpleModel
                    {
                        Id = category.Id,
                        Name = category.GetLocalized(x => x.Name),
                        SeName = category.GetSeName(),
                    }).ToList();
                }
                //manufacturers
                if (_commonSettings.SitemapIncludeManufacturers)
                {
                    var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.MevcutSite.Id);
                    model.Manufacturers = manufacturers.Select(category => new ManufacturerBriefInfoModel
                    {
                        Id = category.Id,
                        Name = category.GetLocalized(x => x.Name),
                        SeName = category.GetSeName(),
                    }).ToList();
                }
                //products
                if (_commonSettings.SitemapIncludeProducts)
                {
                    //limit product to 200 until paging is supported on this page
                    var products = _productService.SearchProducts(storeId: _storeContext.MevcutSite.Id,
                        visibleIndividuallyOnly: true,
                        pageSize: 200);
                    model.Products = products.Select(product => new ProductOverviewModel
                    {
                        Id = product.Id,
                        Name = product.GetLocalized(x => x.Name),
                        ShortDescription = product.GetLocalized(x => x.ShortDescription),
                        FullDescription = product.GetLocalized(x => x.FullDescription),
                        SeName = product.GetSeName(),
                    }).ToList();
                }
                //product tags
                if (_commonSettings.SitemapIncludeProductTags)
                {
                    model.ProductTags = _productTagService.GetAllProductTags().Select(pt => new ProductTagModel
                    {
                        Id = pt.Id,
                        Name = pt.GetLocalized(x => x.Name),
                        SeName = pt.GetSeName()
                    }).ToList();
                }

                //topics
                var topics = _topicService.GetAllTopics(_storeContext.MevcutSite.Id)
                    .Where(t => t.IncludeInSitemap)
                    .ToList();
                model.Topics = topics.Select(topic => new TopicModel
                {
                    Id = topic.Id,
                    SystemName = topic.SystemName,
                    IncludeInSitemap = topic.IncludeInSitemap,
                    IsPasswordProtected = topic.IsPasswordProtected,
                    Title = topic.GetLocalized(x => x.Title),
                })
                .ToList();
                return model;
            });

            return cachedModel;
        }

        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        public virtual string PrepareSitemapXml(int? id)
        {
            var cacheKey = string.Format(ModelÖnbellekOlayTüketici.SITEMAP_SEO_MODEL_KEY, id,
                _workContext.MevcutDil.Id,
                string.Join(",", _workContext.MevcutKullanıcı.GetCustomerRoleIds()),
                _storeContext.MevcutSite.Id);
            var siteMap = _cacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(id));
            return siteMap;
        }

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>Store theme selector model</returns>
        public virtual StoreThemeSelectorModel PrepareStoreThemeSelectorModel()
        {
            var model = new StoreThemeSelectorModel();

            var currentTheme = _themeProvider.GetThemeBySystemName(_themeContext.MevcutTemaAdı);
            model.MevcutSiteTheme = new StoreThemeModel
            {
                Name = currentTheme?.SystemName,
                Title = currentTheme?.FriendlyName
            };

            model.AvailableStoreThemes = _themeProvider.GetThemes().Select(x => new StoreThemeModel
            {
                Name = x.SystemName,
                Title = x.FriendlyName
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        public virtual FaviconModel PrepareFaviconModel()
        {
            var model = new FaviconModel();

            //try loading a store specific favicon

            var faviconFileName = $"favicon-{_storeContext.MevcutSite.Id}.ico";
            var localFaviconPath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, faviconFileName);
                if (!System.IO.File.Exists(localFaviconPath))
                {
                    return model;
                }
            }

            model.FaviconUrl = _webHelper.GetStoreLocation() + faviconFileName;
            return model;
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        public virtual string PrepareRobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            var robotsFilePath = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.custom.txt");
            if (System.IO.File.Exists(robotsFilePath))
            {
                //the robots.txt file exists
                var robotsFileContent = System.IO.File.ReadAllText(robotsFilePath);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/admin",
                    "/bin/",
                    "/files/",
                    "/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/setproductreviewhelpfulness",
                };
                var localizableDisallowPaths = new List<string>
                {
                    "/addproducttocart/catalog/",
                    "/addproducttocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/compareproducts",
                    "/compareproducts/add/*",
                    "/customer/avatar",
                    "/customer/activation",
                    "/customer/addresses",
                    "/customer/changepassword",
                    "/customer/checkusernameavailability",
                    "/customer/downloadableproducts",
                    "/customer/info",
                    "/deletepm",
                    "/emailwishlist",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfilecheckoutattribute",
                    "/uploadfileproductattribute",
                    "/uploadfilereturnrequest",
                    "/wishlist",
                };

                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //sitemaps
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.MevcutSite.Id))
                    {
                        sb.AppendFormat("Sitemap: {0}{1}/sitemap.xml", _storeContext.MevcutSite.Url, language.UniqueSeoCode);
                        sb.Append(newLine);
                    }
                }
                else
                {
                    //localizable paths (without SEO code)
                    sb.AppendFormat("Sitemap: {0}sitemap.xml", _storeContext.MevcutSite.Url);
                    sb.Append(newLine);
                }

                //usual paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                //localizable paths (without SEO code)
                foreach (var path in localizableDisallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.MevcutSite.Id))
                    {
                        foreach (var path in localizableDisallowPaths)
                        {
                            sb.AppendFormat("Disallow: /{0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.additions.txt");
                if (System.IO.File.Exists(robotsAdditionsFile))
                {
                    var robotsFileContent = System.IO.File.ReadAllText(robotsAdditionsFile);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }
        */
        #endregion
    }
}
