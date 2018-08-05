using Core;
using System;
using System.Linq;
using System.Web;
using Core.Domain.Kullanıcılar;
//using Core.Domain.Directory;
//using Core.Domain.Localization;
//using Core.Domain.Tax;
//using Core.Domain.Vendors;
//using Services.Authentication;
using Services.Genel;
using Services.Kullanıcılar;
//using Services.Directory;
using Services.Yardımcılar;
using Services.KimlikDoğrulama;
using Microsoft.AspNetCore.Http;
using Services.Localization;
using Core.Domain.Localization;
using Services.Siteler;
//using Services.Vendors;
using Web.Framework.Localization;
using Microsoft.AspNetCore.Localization;

namespace Web.Framework
{
    public partial class WebWorkContext : IWorkContext
    {
        #region Const

        private const string KullanıcıÇerezAdı = "TS.Kullanici";

        #endregion

        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKullanıcıServisi _KullanıcıService;
        private readonly ISiteContext _storeContext;
        private readonly IGenelÖznitelikServisi _genericAttributeService;
        private readonly IKullanıcıAracıYardımcısı _userAgentHelper;
        private readonly IKimlikDoğrulamaServisi _kimlikDoğrulamaServisi;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILanguageService _languageService;
        private readonly ISiteMappingServisi _siteMappingService;

        private Kullanıcı _önbelleklenmişKullanıcı;
        private Kullanıcı _originalKullanıcıIfImpersonated;
        private Dil _önbelleklenmişDil;

        #endregion

        #region Ctor

        public WebWorkContext(IHttpContextAccessor httpContextAccessor,
            IKullanıcıServisi KullanıcıService,
            ISiteContext storeContext,
            IGenelÖznitelikServisi genericAttributeService,
            IKullanıcıAracıYardımcısı userAgentHelper,
            IKimlikDoğrulamaServisi kimlikDoğrulamaServisi,
            LocalizationSettings localizationSettings,
            ILanguageService languageService,
            ISiteMappingServisi siteMappingService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._KullanıcıService = KullanıcıService;
            this._storeContext = storeContext;
            this._genericAttributeService = genericAttributeService;
            this._userAgentHelper = userAgentHelper;
            this._kimlikDoğrulamaServisi = kimlikDoğrulamaServisi;
            this._localizationSettings = localizationSettings;
            this._languageService = languageService;
            this._siteMappingService = siteMappingService;
        }

        #endregion

        #region Utilities

        protected virtual string KullanıcıÇereziniAl()
        {
            return _httpContextAccessor.HttpContext?.Request?.Cookies[KullanıcıÇerezAdı];
        }
        protected virtual void KullanıcıÇereziniAyarla(Guid KullanıcıGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            _httpContextAccessor.HttpContext.Response.Cookies.Delete(KullanıcıÇerezAdı);

            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            if (KullanıcıGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            var options = new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(KullanıcıÇerezAdı, KullanıcıGuid.ToString(), options);
        }

        public virtual Dil MevcutDil
        {
            get
            {
                if (_önbelleklenmişDil != null)
                    return _önbelleklenmişDil;

                Dil detectedLanguage = null;

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    detectedLanguage = GetLanguageFromUrl();

                if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                {
                    var alreadyDetected = this.MevcutKullanıcı.ÖznitelikAl<bool>(SistemKullanıcıÖznitelikAdları.DilOtomatikAlgılandı,
                        _genericAttributeService, _storeContext.MevcutSite.Id);
                    
                    if (!alreadyDetected)
                    {
                        detectedLanguage = GetLanguageFromRequest();
                        if (detectedLanguage != null)
                        {
                            _genericAttributeService.ÖznitelikKaydet(this.MevcutKullanıcı,
                                SistemKullanıcıÖznitelikAdları.DilOtomatikAlgılandı, true, _storeContext.MevcutSite.Id);
                        }
                    }
                }
                
                if (detectedLanguage != null)
                {
                    //get current saved language identifier
                    var currentLanguageId = this.MevcutKullanıcı.ÖznitelikAl<int>(SistemKullanıcıÖznitelikAdları.LanguageId,
                        _genericAttributeService, _storeContext.MevcutSite.Id);

                    //save the detected language identifier if it differs from the current one
                    if (detectedLanguage.Id != currentLanguageId)
                    {
                        _genericAttributeService.ÖznitelikKaydet(this.MevcutKullanıcı,
                            SistemKullanıcıÖznitelikAdları.LanguageId, detectedLanguage.Id, _storeContext.MevcutSite.Id);
                    }
                }
                
                var customerLanguageId = this.MevcutKullanıcı.ÖznitelikAl<int>(SistemKullanıcıÖznitelikAdları.LanguageId,
                    _genericAttributeService, _storeContext.MevcutSite.Id);

                var allStoreLanguages = _languageService.GetAllLanguages(storeId: _storeContext.MevcutSite.Id);
                var customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == customerLanguageId);
                if (customerLanguage == null)
                {
                    customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == _storeContext.MevcutSite.VarsayılanDilId);
                }
                if (customerLanguage == null)
                    customerLanguage = allStoreLanguages.FirstOrDefault();
                
                if (customerLanguage == null)
                    customerLanguage = _languageService.GetAllLanguages().FirstOrDefault();
                
                _önbelleklenmişDil = customerLanguage;

                return _önbelleklenmişDil;
            }
            set
            {
                var languageId = value?.Id ?? 0;
                _genericAttributeService.ÖznitelikKaydet(this.MevcutKullanıcı,
                    SistemKullanıcıÖznitelikAdları.LanguageId, languageId, _storeContext.MevcutSite.Id);
                _önbelleklenmişDil = null;
            }
        }
        protected virtual Dil GetLanguageFromUrl()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;
            
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            if (!path.IsLocalizedUrl(_httpContextAccessor.HttpContext.Request.PathBase, false, out Dil language))
                return null;

            //check language availability
            if (!_siteMappingService.YetkiVer(language))
                return null;

            return language;
        }
        protected virtual Dil GetLanguageFromRequest()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;
            
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            if (requestCulture == null)
                return null;
            
            var requestLanguage = _languageService.GetAllLanguages().FirstOrDefault(language =>
                language.DilKültürü.Equals(requestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));
            
            if (requestLanguage == null || !requestLanguage.Yayınlandı || !_siteMappingService.YetkiVer(requestLanguage))
                return null;

            return requestLanguage;
        }

        public virtual Kullanıcı MevcutKullanıcı
        {
            get
            {
                //whether there is a cached value
                if (_önbelleklenmişKullanıcı != null)
                    return _önbelleklenmişKullanıcı;

                Kullanıcı customer = null;

                //check whether request is made by a background (schedule) task
                if (_httpContextAccessor.HttpContext == null )
                {
                    //in this case return built-in customer record for background task
                    customer = _KullanıcıService.KullanıcıAlSistemAdı(SistemKullanıcıAdları.ArkaPlanGörevi);
                }

                if (customer == null || customer.Silindi || !customer.Aktif || customer.GirişGerekli)
                {
                    //check whether request is made by a search engine, in this case return built-in customer record for search engines
                    if (_userAgentHelper.AramaMotoru())
                        customer = _KullanıcıService.KullanıcıAlSistemAdı(SistemKullanıcıAdları.AramaMotoru);
                }

                if (customer == null || customer.Silindi || !customer.Aktif || customer.GirişGerekli)
                {
                    //try to get registered user
                    customer = _kimlikDoğrulamaServisi.KimliğiDoğrulananKullanıcı();
                }

                if (customer != null && !customer.Silindi && customer.Aktif && !customer.GirişGerekli)
                {
                    //get impersonate user if required
                    var impersonatedKullanıcıId = customer.ÖznitelikAl<int?>(SistemKullanıcıÖznitelikAdları.KimliğineBürünülenKullanıcıId);
                    if (impersonatedKullanıcıId.HasValue && impersonatedKullanıcıId.Value > 0)
                    {
                        var impersonatedKullanıcı = _KullanıcıService.KullanıcıAlId(impersonatedKullanıcıId.Value);
                        if (impersonatedKullanıcı != null && !impersonatedKullanıcı.Silindi && impersonatedKullanıcı.Aktif && !impersonatedKullanıcı.GirişGerekli)
                        {
                            //set impersonated customer
                            _originalKullanıcıIfImpersonated = customer;
                            customer = impersonatedKullanıcı;
                        }
                    }
                }

                if (customer == null || customer.Silindi || !customer.Aktif || customer.GirişGerekli)
                {
                    //get guest customer
                    var customerCookie = KullanıcıÇereziniAl();
                    if (!string.IsNullOrEmpty(customerCookie))
                    {
                        if (Guid.TryParse(customerCookie, out Guid customerGuid))
                        {
                            //get customer from cookie (should not be registered)
                            var customerByCookie = _KullanıcıService.KullanıcıAlGuid(customerGuid);
                            if (customerByCookie != null && !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                if (customer == null || customer.Silindi || !customer.Aktif || customer.GirişGerekli)
                {
                    //create guest if not exists
                    customer = _KullanıcıService.ZiyaretciKullanıcıEkle();
                }

                if (!customer.Silindi && customer.Aktif && !customer.GirişGerekli)
                {
                    KullanıcıÇereziniAyarla(customer.KullanıcıGuid);

                    _önbelleklenmişKullanıcı = customer;
                }

                return _önbelleklenmişKullanıcı;
            }
            set
            {
                KullanıcıÇereziniAyarla(value.KullanıcıGuid);
                _önbelleklenmişKullanıcı = value;
            }
        }

        public virtual Kullanıcı OrijinalKullanıcıyıTaklitEt
        {
            get
            {
                return _originalKullanıcıIfImpersonated;
            }
        }
        public virtual bool Yönetici { get; set; }

        #endregion
    }
}