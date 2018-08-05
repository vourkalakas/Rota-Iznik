using Core.Önbellek;
using Core.Domain.Blogs;
using Core.Domain.Katalog;
using Core.Domain.Yapılandırma;
using Core.Domain.Klasör;
using Core.Domain.Localization;
using Core.Domain.Medya;
using Core.Domain.Haber;
using Core.Domain.Siparişler;
using Core.Domain.Anket;
using Core.Domain.Sayfalar;
using Core.Olaylar;
using Services.Olaylar;

namespace Web.Altyapı.Önbellek
{
    public partial class ModelÖnbellekOlayTüketici :
        //languages
        IMüşteri<OlayEklendi<Dil>>,
        IMüşteri<OlayGüncellendi<Dil>>,
        IMüşteri<OlaySilindi<Dil>>
        //currencies
        /*IMüşteri<OlayEklendi<Currency>>,
        IMüşteri<OlayGüncellendi<Currency>>,
        IMüşteri<OlaySilindi<Currency>>,
        //settings
        IMüşteri<OlayGüncellendi<Setting>>,
        //manufacturers
        IMüşteri<OlayEklendi<Manufacturer>>,
        IMüşteri<OlayGüncellendi<Manufacturer>>,
        IMüşteri<OlaySilindi<Manufacturer>>,
        //vendors
        IMüşteri<OlayEklendi<Vendor>>,
        IMüşteri<OlayGüncellendi<Vendor>>,
        IMüşteri<OlaySilindi<Vendor>>,
        //product manufacturers
        IMüşteri<OlayEklendi<ProductManufacturer>>,
        IMüşteri<OlayGüncellendi<ProductManufacturer>>,
        IMüşteri<OlaySilindi<ProductManufacturer>>,
        //categories
        IMüşteri<OlayEklendi<Category>>,
        IMüşteri<OlayGüncellendi<Category>>,
        IMüşteri<OlaySilindi<Category>>,
        //product categories
        IMüşteri<OlayEklendi<ProductCategory>>,
        IMüşteri<OlayGüncellendi<ProductCategory>>,
        IMüşteri<OlaySilindi<ProductCategory>>,
        //products
        IMüşteri<OlayEklendi<Product>>,
        IMüşteri<OlayGüncellendi<Product>>,
        IMüşteri<OlaySilindi<Product>>,
        //related product
        IMüşteri<OlayEklendi<RelatedProduct>>,
        IMüşteri<OlayGüncellendi<RelatedProduct>>,
        IMüşteri<OlaySilindi<RelatedProduct>>,
        //product tags
        IMüşteri<OlayEklendi<ProductTag>>,
        IMüşteri<OlayGüncellendi<ProductTag>>,
        IMüşteri<OlaySilindi<ProductTag>>,
        //specification attributes
        IMüşteri<OlayGüncellendi<SpecificationAttribute>>,
        IMüşteri<OlaySilindi<SpecificationAttribute>>,
        //specification attribute options
        IMüşteri<OlayGüncellendi<SpecificationAttributeOption>>,
        IMüşteri<OlaySilindi<SpecificationAttributeOption>>,
        //Product specification attribute
        IMüşteri<OlayEklendi<ProductSpecificationAttribute>>,
        IMüşteri<OlayGüncellendi<ProductSpecificationAttribute>>,
        IMüşteri<OlaySilindi<ProductSpecificationAttribute>>,
        //Product attributes
        IMüşteri<OlaySilindi<ProductAttribute>>,
        //Product attributes
        IMüşteri<OlayEklendi<ProductAttributeMapping>>,
        IMüşteri<OlaySilindi<ProductAttributeMapping>>,
        //Product attribute values
        IMüşteri<OlayGüncellendi<ProductAttributeValue>>,
        //Topics
        IMüşteri<OlayEklendi<Topic>>,
        IMüşteri<OlayGüncellendi<Topic>>,
        IMüşteri<OlaySilindi<Topic>>,
        //Orders
        IMüşteri<OlayEklendi<Order>>,
        IMüşteri<OlayGüncellendi<Order>>,
        IMüşteri<OlaySilindi<Order>>,
        //Picture
        IMüşteri<OlayEklendi<Picture>>,
        IMüşteri<OlayGüncellendi<Picture>>,
        IMüşteri<OlaySilindi<Picture>>,
        //Product picture mapping
        IMüşteri<OlayEklendi<ProductPicture>>,
        IMüşteri<OlayGüncellendi<ProductPicture>>,
        IMüşteri<OlaySilindi<ProductPicture>>,
        //Product review
        IMüşteri<OlaySilindi<ProductReview>>,
        //polls
        IMüşteri<OlayEklendi<Poll>>,
        IMüşteri<OlayGüncellendi<Poll>>,
        IMüşteri<OlaySilindi<Poll>>,
        //blog posts
        IMüşteri<OlayEklendi<BlogPost>>,
        IMüşteri<OlayGüncellendi<BlogPost>>,
        IMüşteri<OlaySilindi<BlogPost>>,
        //blog comments
        IMüşteri<OlaySilindi<BlogComment>>,
        //news items
        IMüşteri<OlayEklendi<NewsItem>>,
        IMüşteri<OlayGüncellendi<NewsItem>>,
        IMüşteri<OlaySilindi<NewsItem>>,
        //news comments
        IMüşteri<OlaySilindi<NewsComment>>,
        //states/province
        IMüşteri<OlayEklendi<StateProvince>>,
        IMüşteri<OlayGüncellendi<StateProvince>>,
        IMüşteri<OlaySilindi<StateProvince>>,
        //return requests
        IMüşteri<OlayEklendi<ReturnRequestAction>>,
        IMüşteri<OlayGüncellendi<ReturnRequestAction>>,
        IMüşteri<OlaySilindi<ReturnRequestAction>>,
        IMüşteri<OlayEklendi<ReturnRequestReason>>,
        IMüşteri<OlayGüncellendi<ReturnRequestReason>>,
        IMüşteri<OlaySilindi<ReturnRequestReason>>,
        //templates
        IMüşteri<OlayEklendi<CategoryTemplate>>,
        IMüşteri<OlayGüncellendi<CategoryTemplate>>,
        IMüşteri<OlaySilindi<CategoryTemplate>>,
        IMüşteri<OlayEklendi<ManufacturerTemplate>>,
        IMüşteri<OlayGüncellendi<ManufacturerTemplate>>,
        IMüşteri<OlaySilindi<ManufacturerTemplate>>,
        IMüşteri<OlayEklendi<ProductTemplate>>,
        IMüşteri<OlayGüncellendi<ProductTemplate>>,
        IMüşteri<OlaySilindi<ProductTemplate>>,
        IMüşteri<OlayEklendi<TopicTemplate>>,
        IMüşteri<OlayGüncellendi<TopicTemplate>>,
        IMüşteri<OlaySilindi<TopicTemplate>>,
        //checkout attributes
        IMüşteri<OlayEklendi<CheckoutAttribute>>,
        IMüşteri<OlayGüncellendi<CheckoutAttribute>>,
        IMüşteri<OlaySilindi<CheckoutAttribute>>,
        //shopping cart items
        IMüşteri<OlayGüncellendi<ShoppingCartItem>>*/
    {
        #region Fields

        private readonly KatalogAyarları _catalogSettings;
        private readonly IStatikÖnbellekYönetici _cacheManager;

        #endregion

        #region Ctor

        public ModelÖnbellekOlayTüketici(KatalogAyarları catalogSettings, IStatikÖnbellekYönetici cacheManager)
        {
            this._cacheManager = cacheManager;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Cache keys 
        public const string SEARCH_CATEGORIES_MODEL_KEY = "pres.search.categories-{0}-{1}-{2}";
        public const string SEARCH_CATEGORIES_PATTERN_KEY = "pres.search.categories";
        public const string MANUFACTURER_NAVIGATION_MODEL_KEY = "pres.manufacturer.navigation-{0}-{1}-{2}-{3}";
        public const string MANUFACTURER_NAVIGATION_PATTERN_KEY = "pres.manufacturer.navigation";
        public const string VENDOR_NAVIGATION_MODEL_KEY = "pres.vendor.navigation";
        public const string VENDOR_NAVIGATION_PATTERN_KEY = "pres.vendor.navigation";
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY = "pres.manufacturer.hasfeaturedproducts-{0}-{1}-{2}";
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "pres.manufacturer.hasfeaturedproducts";
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID = "pres.manufacturer.hasfeaturedproducts-{0}-";
        public const string CATEGORY_ALL_MODEL_KEY = "pres.category.all-{0}-{1}-{2}";
        public const string CATEGORY_ALL_PATTERN_KEY = "pres.category.all";
        public const string CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY = "pres.category.numberofproducts-{0}-{1}-{2}";
        public const string CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY = "pres.category.numberofproducts";
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_KEY = "pres.category.hasfeaturedproducts-{0}-{1}-{2}";
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "pres.category.hasfeaturedproducts";
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID = "pres.category.hasfeaturedproducts-{0}-";
        public const string CATEGORY_BREADCRUMB_KEY = "pres.category.breadcrumb-{0}-{1}-{2}-{3}";
        public const string CATEGORY_BREADCRUMB_PATTERN_KEY = "pres.category.breadcrumb";
        public const string CATEGORY_SUBCATEGORIES_KEY = "pres.category.subcategories-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_SUBCATEGORIES_PATTERN_KEY = "pres.category.subcategories";
        public const string CATEGORY_HOMEPAGE_KEY = "pres.category.homepage-{0}-{1}-{2}-{3}-{4}";
        public const string CATEGORY_HOMEPAGE_PATTERN_KEY = "pres.category.homepage";
        public const string CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY = "pres.category.childidentifiers-{0}-{1}-{2}";
        public const string CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY = "pres.category.childidentifiers";
        public const string SPECS_FILTER_MODEL_KEY = "pres.filter.specs-{0}-{1}";
        public const string SPECS_FILTER_PATTERN_KEY = "pres.filter.specs";
        public const string PRODUCT_BREADCRUMB_MODEL_KEY = "pres.product.breadcrumb-{0}-{1}-{2}-{3}";
        public const string PRODUCT_BREADCRUMB_PATTERN_KEY = "pres.product.breadcrumb";
        public const string PRODUCT_BREADCRUMB_PATTERN_KEY_BY_ID = "pres.product.breadcrumb-{0}-";
        public const string PRODUCTTAG_BY_PRODUCT_MODEL_KEY = "pres.producttag.byproduct-{0}-{1}-{2}";
        public const string PRODUCTTAG_BY_PRODUCT_PATTERN_KEY = "pres.producttag.byproduct";
        public const string PRODUCTTAG_POPULAR_MODEL_KEY = "pres.producttag.popular-{0}-{1}";
        public const string PRODUCTTAG_POPULAR_PATTERN_KEY = "pres.producttag.popular";
        public const string PRODUCT_MANUFACTURERS_MODEL_KEY = "pres.product.manufacturers-{0}-{1}-{2}-{3}";
        public const string PRODUCT_MANUFACTURERS_PATTERN_KEY = "pres.product.manufacturers";
        public const string PRODUCT_MANUFACTURERS_PATTERN_KEY_BY_ID = "pres.product.manufacturers-{0}-";
        public const string PRODUCT_SPECS_MODEL_KEY = "pres.product.specs-{0}-{1}";
        public const string PRODUCT_SPECS_PATTERN_KEY = "pres.product.specs";
        public const string PRODUCT_SPECS_PATTERN_KEY_BY_ID = "pres.product.specs-{0}-";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY = "pres.product.hasproductattributes-{0}-";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY = "pres.product.hasproductattributes";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY_BY_ID = "pres.product.hasproductattributes-{0}-";
        public const string TOPIC_MODEL_BY_SYSTEMNAME_KEY = "pres.topic.details.bysystemname-{0}-{1}-{2}-{3}";
        public const string TOPIC_MODEL_BY_ID_KEY = "pres.topic.details.byid-{0}-{1}-{2}-{3}";
        public const string TOPIC_SENAME_BY_SYSTEMNAME = "pres.topic.sename.bysystemname-{0}-{1}-{2}";
        public const string TOPIC_TITLE_BY_SYSTEMNAME = "pres.topic.title.bysystemname-{0}-{1}-{2}";
        public const string TOPIC_TOP_MENU_MODEL_KEY = "pres.topic.topmenu-{0}-{1}-{2}";
        public const string TOPIC_FOOTER_MODEL_KEY = "pres.topic.footer-{0}-{1}-{2}";
        public const string TOPIC_PATTERN_KEY = "pres.topic";
        public const string CATEGORY_TEMPLATE_MODEL_KEY = "pres.categorytemplate-{0}";
        public const string CATEGORY_TEMPLATE_PATTERN_KEY = "pres.categorytemplate";
        public const string MANUFACTURER_TEMPLATE_MODEL_KEY = "pres.manufacturertemplate-{0}";
        public const string MANUFACTURER_TEMPLATE_PATTERN_KEY = "pres.manufacturertemplate";
        public const string PRODUCT_TEMPLATE_MODEL_KEY = "pres.producttemplate-{0}";
        public const string PRODUCT_TEMPLATE_PATTERN_KEY = "pres.producttemplate";
        public const string TOPIC_TEMPLATE_MODEL_KEY = "pres.topictemplate-{0}";
        public const string TOPIC_TEMPLATE_PATTERN_KEY = "pres.topictemplate";
        public const string HOMEPAGE_BESTSELLERS_IDS_KEY = "pres.bestsellers.homepage-{0}";
        public const string HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY = "pres.bestsellers.homepage";
        public const string PRODUCTS_ALSO_PURCHASED_IDS_KEY = "pres.alsopuchased-{0}-{1}";
        public const string PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY = "pres.alsopuchased";
        public const string PRODUCTS_RELATED_IDS_KEY = "pres.related-{0}-{1}";
        public const string PRODUCTS_RELATED_IDS_PATTERN_KEY = "pres.related";
        public const string PRODUCT_DEFAULTPICTURE_MODEL_KEY = "pres.product.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DEFAULTPICTURE_PATTERN_KEY = "pres.product.detailspictures";
        public const string PRODUCT_DEFAULTPICTURE_PATTERN_KEY_BY_ID = "pres.product.detailspictures-{0}-";
        public const string PRODUCT_DETAILS_PICTURES_MODEL_KEY = "pres.product.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DETAILS_PICTURES_PATTERN_KEY = "pres.product.picture";
        public const string PRODUCT_DETAILS_PICTURES_PATTERN_KEY_BY_ID = "pres.product.picture-{0}-";
        public const string PRODUCT_REVIEWS_MODEL_KEY = "pres.product.reviews-{0}-{1}";
        public const string PRODUCT_REVIEWS_PATTERN_KEY = "pres.product.reviews";
        public const string PRODUCT_REVIEWS_PATTERN_KEY_BY_ID = "pres.product.reviews-{0}-";
        public const string PRODUCTATTRIBUTE_PICTURE_MODEL_KEY = "pres.productattribute.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY = "pres.productattribute.picture";
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY = "pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY = "pres.productattribute.imagesquare.picture";
        public const string CATEGORY_PICTURE_MODEL_KEY = "pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_PICTURE_PATTERN_KEY = "pres.category.picture";
        public const string CATEGORY_PICTURE_PATTERN_KEY_BY_ID = "pres.category.picture-{0}-";
        public const string MANUFACTURER_PICTURE_MODEL_KEY = "pres.manufacturer.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string MANUFACTURER_PICTURE_PATTERN_KEY = "pres.manufacturer.picture";
        public const string MANUFACTURER_PICTURE_PATTERN_KEY_BY_ID = "pres.manufacturer.picture-{0}-";
        public const string VENDOR_PICTURE_MODEL_KEY = "pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string VENDOR_PICTURE_PATTERN_KEY = "pres.vendor.picture";
        public const string VENDOR_PICTURE_PATTERN_KEY_BY_ID = "pres.vendor.picture-{0}-";
        public const string CART_PICTURE_MODEL_KEY = "pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CART_PICTURE_PATTERN_KEY = "pres.cart.picture";
        public const string HOMEPAGE_POLLS_MODEL_KEY = "pres.poll.homepage-{0}";
        public const string POLL_BY_SYSTEMNAME_MODEL_KEY = "pres.poll.systemname-{0}-{1}";
        public const string POLLS_PATTERN_KEY = "pres.poll";
        public const string BLOG_TAGS_MODEL_KEY = "pres.blog.tags-{0}-{1}";
        public const string BLOG_MONTHS_MODEL_KEY = "pres.blog.months-{0}-{1}";
        public const string BLOG_PATTERN_KEY = "pres.blog";
        public const string BLOG_COMMENTS_NUMBER_KEY = "pres.blog.comments.number-{0}-{1}-{2}";
        public const string BLOG_COMMENTS_PATTERN_KEY = "pres.blog.comments";
        public const string HOMEPAGE_NEWSMODEL_KEY = "pres.news.homepage-{0}-{1}";
        public const string NEWS_PATTERN_KEY = "pres.news";
        public const string NEWS_COMMENTS_NUMBER_KEY = "pres.news.comments.number-{0}-{1}-{2}";
        public const string NEWS_COMMENTS_PATTERN_KEY = "pres.news.comments";
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "pres.stateprovinces";
        public const string RETURNREQUESTREASONS_MODEL_KEY = "pres.returnrequesreasons-{0}";
        public const string RETURNREQUESTREASONS_PATTERN_KEY = "pres.returnrequesreasons";
        public const string RETURNREQUESTACTIONS_MODEL_KEY = "pres.returnrequestactions-{0}";
        public const string RETURNREQUESTACTIONS_PATTERN_KEY = "pres.returnrequestactions";
        public const string STORE_LOGO_PATH = "pres.logo-{0}-{1}-{2}";
        public const string STORE_LOGO_PATH_PATTERN_KEY = "pres.logo";
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "pres.languages.all-{0}";
        public const string AVAILABLE_LANGUAGES_PATTERN_KEY = "pres.languages";
        public const string AVAILABLE_CURRENCIES_MODEL_KEY = "pres.currencies.all-{0}-{1}";
        public const string AVAILABLE_CURRENCIES_PATTERN_KEY = "pres.currencies";
        public const string CHECKOUTATTRIBUTES_EXIST_KEY = "pres.checkoutattributes.exist-{0}-{1}";
        public const string CHECKOUTATTRIBUTES_PATTERN_KEY = "pres.checkoutattributes";
        public const string SITEMAP_PAGE_MODEL_KEY = "pres.sitemap.page-{0}-{1}-{2}";
        public const string SITEMAP_SEO_MODEL_KEY = "pres.sitemap.seo-{0}-{1}-{2}-{3}";
        public const string SITEMAP_PATTERN_KEY = "pres.sitemap";
        public const string WIDGET_MODEL_KEY = "pres.widget-{0}-{1}-{2}-{3}";
        public const string WIDGET_PATTERN_KEY = "pres.widget";

        #endregion

        #region Methods

        //languages
        public void Olay(OlayEklendi<Dil> eventMessage)
        {
            //clear all localizable models
            _cacheManager.KalıpİleSil(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.KalıpİleSil(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.KalıpİleSil(TOPIC_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.KalıpİleSil(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.KalıpİleSil(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Dil> eventMessage)
        {
            //clear all localizable models
            _cacheManager.KalıpİleSil(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.KalıpİleSil(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.KalıpİleSil(TOPIC_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.KalıpİleSil(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.KalıpİleSil(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Dil> eventMessage)
        {
            //clear all localizable models
            _cacheManager.KalıpİleSil(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.KalıpİleSil(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.KalıpİleSil(TOPIC_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.KalıpİleSil(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.KalıpİleSil(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.KalıpİleSil(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.KalıpİleSil(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        /*
        //currencies
        public void Olay(OlayEklendi<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        public void Olay(OlayGüncellendi<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY); //depends on CatalogSettings.NumberOfProductTags
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY); //depends on VendorSettings.VendorBlockItemsToDisplay
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY); //depends on BlogSettings.NumberOfTags
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY); //depends on NewsSettings.MainPageNewsCount
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY); //depends on distinct sitemap settings
            _cacheManager.RemoveByPattern(WIDGET_PATTERN_KEY); //depends on WidgetSettings and certain settings of widgets
            _cacheManager.RemoveByPattern(STORE_LOGO_PATH_PATTERN_KEY); //depends on StoreInformationSettings.LogoPictureId
        }

        //vendors
        public void Olay(OlayEklendi<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(VENDOR_PICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
        }
        public void Olay(OlaySilindi<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }

        //manufacturers
        public void Olay(OlayEklendi<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);

        }
        public void Olay(OlayGüncellendi<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(MANUFACTURER_PICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
        }
        public void Olay(OlaySilindi<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product manufacturers
        public void Olay(OlayEklendi<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_MANUFACTURERS_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID, eventMessage.Entity.ManufacturerId));
        }
        public void Olay(OlayGüncellendi<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_MANUFACTURERS_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID, eventMessage.Entity.ManufacturerId));
        }
        public void Olay(OlaySilindi<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_MANUFACTURERS_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID, eventMessage.Entity.ManufacturerId));
        }

        //categories
        public void Olay(OlayEklendi<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_PICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
        }
        public void Olay(OlaySilindi<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product categories
        public void Olay(OlayEklendi<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_BREADCRUMB_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            }
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID, eventMessage.Entity.CategoryId));
        }
        public void Olay(OlayGüncellendi<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_BREADCRUMB_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID, eventMessage.Entity.CategoryId));
        }
        public void Olay(OlaySilindi<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_BREADCRUMB_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            }
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID, eventMessage.Entity.CategoryId));
        }

        //products
        public void Olay(OlayEklendi<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_REVIEWS_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product tags
        public void Olay(OlayEklendi<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }

        //related products
        public void Olay(OlayEklendi<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }

        //specification attributes
        public void Olay(OlayGüncellendi<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }

        //specification attribute options
        public void Olay(OlayGüncellendi<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }

        //Product specification attribute
        public void Olay(OlayEklendi<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_SPECS_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_SPECS_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_SPECS_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }

        //Product attributes
        public void Olay(OlaySilindi<ProductAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY);
        }
        //Product attributes
        public void Olay(OlayEklendi<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
        }
        public void Olay(OlaySilindi<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
        }
        //Product attributes
        public void Olay(OlayGüncellendi<ProductAttributeValue> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY);
        }

        //Topics
        public void Olay(OlayEklendi<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //Orders
        public void Olay(OlayEklendi<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }

        //Pictures
        public void Olay(OlayEklendi<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Product picture mappings
        public void Olay(OlayEklendi<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_DEFAULTPICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_DETAILS_PICTURES_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_DEFAULTPICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_DETAILS_PICTURES_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_DEFAULTPICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_DETAILS_PICTURES_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Polls
        public void Olay(OlayEklendi<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }

        //Blog posts
        public void Olay(OlayEklendi<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }

        //Blog comments
        public void Olay(OlaySilindi<BlogComment> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_COMMENTS_PATTERN_KEY);
        }

        //News items
        public void Olay(OlayEklendi<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        //News comments
        public void Olay(OlaySilindi<NewsComment> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_COMMENTS_PATTERN_KEY);
        }

        //State/province
        public void Olay(OlayEklendi<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        //return requests
        public void Olay(OlayEklendi<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void Olay(OlayEklendi<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }

        //templates
        public void Olay(OlayEklendi<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlayEklendi<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlayEklendi<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlayEklendi<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }

        //checkout attributes
        public void Olay(OlayEklendi<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }

        //shopping cart items
        public void Olay(OlayGüncellendi<ShoppingCartItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //product reviews
        public void Olay(OlaySilindi<ProductReview> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(PRODUCT_REVIEWS_PATTERN_KEY_BY_ID, eventMessage.Entity.ProductId));
        }
        */
        #endregion
    }
}
