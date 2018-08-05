using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Core;
using Core.Data;
using Core.Domain.Kullanıcılar;
using Core.Eklentiler;
using Services.Genel;
using Services.Kullanıcılar;
using Services.Olaylar;
using Services.Logging;
using Services.Mesajlar;
using Services.KimlikDoğrulama;

namespace Services.KimlikDoğrulama.Harici
{
    public partial class HariciYetkilendirmeServisi : IHariciYetkilendirmeServisi
    {
        #region Fields

        private readonly KullanıcıAyarları _kullanıcıAyarları;
        private readonly HariciYetkilendirmeAyarları _hariciYetkilendirmeAyarları;
        private readonly IKimlikDoğrulamaServisi _yetkilendirmeServisi;
        private readonly IKullanıcıİşlemServisi _kullanıcıİşlemServisi;
        private readonly IKullanıcıKayıtServisi _kullanıcıKayıtServisi;
        private readonly IKullanıcıServisi _kullanıcıServisi;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IGenelÖznitelikServisi _genelÖznitelikServisi;
        //private readonly ILocalizationService _localizationService;
        private readonly IEklentiBulucu _eklentiBulucu;
        private readonly IDepo<HariciKimlikDoğrulamaKaydı> _hariciYetkilendirmeKaydıDepo;
        //private readonly IShoppingCartService _shoppingCartService;
        private readonly ISiteContext _siteContext;
        private readonly IWorkContext _workContext;
        //private readonly IWorkflowMessageService _workflowMessageService;
        //private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor
        public HariciYetkilendirmeServisi(KullanıcıAyarları kullanıcıAyarları,
            HariciYetkilendirmeAyarları hariciYetkilendirmeAyarları,
            IKimlikDoğrulamaServisi yetkilendirmeServisi,
            IKullanıcıİşlemServisi kullanıcıİşlemServisi,
            IKullanıcıKayıtServisi kullanıcıKayıtServisi,
            IKullanıcıServisi kullanıcıServisi,
            IOlayYayınlayıcı olayYayınlayıcı,
            IGenelÖznitelikServisi genelÖznitelikServisi,
            //ILocalizationService localizationService,
            IEklentiBulucu eklentiBulucu,
            IDepo<HariciKimlikDoğrulamaKaydı> hariciYetkilendirmeKaydıDepo,
            //IShoppingCartService shoppingCartService,
            ISiteContext siteContext,
            IWorkContext workContext)
        {
            this._kullanıcıAyarları = kullanıcıAyarları;
            this._hariciYetkilendirmeAyarları = hariciYetkilendirmeAyarları;
            this._yetkilendirmeServisi = yetkilendirmeServisi;
            this._kullanıcıİşlemServisi = kullanıcıİşlemServisi;
            this._kullanıcıKayıtServisi = kullanıcıKayıtServisi;
            this._kullanıcıServisi = kullanıcıServisi;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._genelÖznitelikServisi = genelÖznitelikServisi;
            //this._localizationService = localizationService;
            this._eklentiBulucu = eklentiBulucu;
            this._hariciYetkilendirmeKaydıDepo = hariciYetkilendirmeKaydıDepo;
            //this._shoppingCartService = shoppingCartService;
            this._siteContext = siteContext;
            this._workContext = workContext;
            //this._workflowMessageService = workflowMessageService;
            //this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities
        protected virtual IActionResult AuthenticateExistingUser(Kullanıcı associatedUser, Kullanıcı currentLoggedInUser, string returnUrl)
        {
            if (currentLoggedInUser == null)
                return LoginUser(associatedUser, returnUrl);
            
            if (currentLoggedInUser.Id != associatedUser.Id)
                return ErrorAuthentication(new[] { "Account is already assigned" }, returnUrl);

            return SuccessfulAuthentication(returnUrl);
        }
        
        protected virtual IActionResult AuthenticateNewUser(Kullanıcı currentLoggedInUser, HariciYetkilendirmeParametreleri parameters, string returnUrl)
        {
            if (currentLoggedInUser != null)
            {
                HariciHesabıKullanıcıylaİlişkilendir(currentLoggedInUser, parameters);
                return SuccessfulAuthentication(returnUrl);
            }
            if (_kullanıcıAyarları.KullanıcıKayıtTipi != KullanıcıKayıtTipi.Engelli)
                return RegisterNewUser(parameters, returnUrl);

            return ErrorAuthentication(new[] { "Registration is disabled" }, returnUrl);
        }
        protected virtual IActionResult RegisterNewUser(HariciYetkilendirmeParametreleri parameters, string returnUrl)
        {
            //check whether the specified email has been already registered
            if (_kullanıcıServisi.KullanıcıAlEmail(parameters.Email) != null)
            {
                var alreadyExistsError = "E-Mail adresi zaten mevcut";
                return ErrorAuthentication(new[] { alreadyExistsError }, returnUrl);
            }
            
            var registrationIsApproved = _kullanıcıAyarları.KullanıcıKayıtTipi == KullanıcıKayıtTipi.Standart ||
                (_kullanıcıAyarları.KullanıcıKayıtTipi == KullanıcıKayıtTipi.EmailDoğrulaması && !_hariciYetkilendirmeAyarları.EmailDoğrulamasıGerekli);
            
            var registrationRequest = new KullanıcıKayıtİsteği(_workContext.MevcutKullanıcı,
                parameters.Email, parameters.Email,
                GenelYardımcı.RastgeleOndalıklıÜret(20),
                ŞifreFormatı.Temiz,
                _siteContext.MevcutSite.Id,
                registrationIsApproved);
            
            var registrationResult = _kullanıcıKayıtServisi.KullanıcıKaydet(registrationRequest);
            if (!registrationResult.Başarılı)
                return ErrorAuthentication(registrationResult.Hatalar, returnUrl);
            
            _olayYayınlayıcı.Yayınla(new KullanıcıHariciOlarakOtomatikKaydedildi(_workContext.MevcutKullanıcı, parameters));
            
            _olayYayınlayıcı.Yayınla(new KullanıcıKaydolduOlayı(_workContext.MevcutKullanıcı));
            
            if (_kullanıcıAyarları.YeniKullanıcıBildirimi)
                //_workflowMessageService.SendCustomerRegisteredNotificationMessage(_workContext.MevcutKullanıcı, _localizationSettings.DefaultAdminLanguageId);
                
            HariciHesabıKullanıcıylaİlişkilendir(_workContext.MevcutKullanıcı, parameters);

            //yetkilendir
            if (registrationIsApproved)
            {
                _yetkilendirmeServisi.Giriş(_workContext.MevcutKullanıcı, false);
                //_workflowMessageService.SendCustomerWelcomeMessage(_workContext.MevcutKullanıcı, _workContext.WorkingLanguage.Id);

                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)KullanıcıKayıtTipi.Standart });
            }

            //registration is succeeded but isn't activated
            if (_kullanıcıAyarları.KullanıcıKayıtTipi == KullanıcıKayıtTipi.EmailDoğrulaması)
            {
                //email validation message
                _genelÖznitelikServisi.ÖznitelikKaydet(_workContext.MevcutKullanıcı, SistemKullanıcıÖznitelikAdları.HesapAktifleştirmeKodu, Guid.NewGuid().ToString());
                //_workflowMessageService.SendCustomerEmailValidationMessage(_workContext.MevcutKullanıcı, _workContext.WorkingLanguage.Id);

                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)KullanıcıKayıtTipi.EmailDoğrulaması });
            }
            
            if (_kullanıcıAyarları.KullanıcıKayıtTipi == KullanıcıKayıtTipi.YöneticiOnayı)
                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)KullanıcıKayıtTipi.YöneticiOnayı });
            
            return ErrorAuthentication(new[] { "Error on registration" }, returnUrl);
        }
        protected virtual IActionResult LoginUser(Kullanıcı user, string returnUrl)
        {
            //migrate shopping cart
            //_shoppingCartService.MigrateShoppingCart(_workContext.MevcutKullanıcı, user, true);

            //authenticate
            _yetkilendirmeServisi.Giriş(user, false);

            //raise event       
            _olayYayınlayıcı.Yayınla(new KullanıcıBağlandıOlayı(user));

            //activity log
            _kullanıcıİşlemServisi.İşlemEkle("Kullanıcı giriş yaptı "+ user.KullanıcıAdı,"");

            return SuccessfulAuthentication(returnUrl);
        }
        protected virtual IActionResult ErrorAuthentication(IEnumerable<string> errors, string returnUrl)
        {
            foreach (var error in errors)
                HariciYetkilendiriciYardımcısı.AddErrorsToDisplay(error);

            return new RedirectToActionResult("Login", "Kullanıcı", !string.IsNullOrEmpty(returnUrl) ? new { ReturnUrl = returnUrl } : null);
        }
        protected virtual IActionResult SuccessfulAuthentication(string returnUrl)
        {
            //redirect to the return URL if it's specified
            if (!string.IsNullOrEmpty(returnUrl))
                return new RedirectResult(returnUrl);

            return new RedirectToRouteResult("Anasayfa", null);
        }

        #endregion

        #region Methods

        #region External authentication methods
        
        public virtual IList<IHariciYetkilendirmeMetodu> AktifHariciYetkilendirmeMetodlarıYükle(Kullanıcı customer = null, int storeId = 0)
        {
            return TümAktifHariciYetkilendirmeMetodlarıYükle(customer, storeId)
                .Where(provider => _hariciYetkilendirmeAyarları.AktifYetkilendirmeMetoduSistemAdları
                    .Contains(provider.EklentiTanımlayıcı.SistemAdı, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }
        public virtual IHariciYetkilendirmeMetodu AktifHariciYetkilendirmeMetoduYükleSistemAdı(string systemName)
        {
            var descriptor = _eklentiBulucu.EklentiTanımlayıcıAlSistemAdı<IHariciYetkilendirmeMetodu>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IHariciYetkilendirmeMetodu>();

            return null;
        }
        public virtual IList<IHariciYetkilendirmeMetodu> TümAktifHariciYetkilendirmeMetodlarıYükle(Kullanıcı kullanıcı = null, int siteId = 0)
        {
            return _eklentiBulucu.EklentileriAl<IHariciYetkilendirmeMetodu>(kullanıcı: kullanıcı, siteId: siteId).ToList();
        }
        public virtual bool HariciYetkilendirmeMetoduErişilebilir(string sistemAdı)
        {
            //load method
            var authenticationMethod = AktifHariciYetkilendirmeMetoduYükleSistemAdı(sistemAdı);

            return authenticationMethod != null &&
                authenticationMethod.MetodAktif(_hariciYetkilendirmeAyarları) &&
                authenticationMethod.EklentiTanımlayıcı.Kuruldu &&
                _eklentiBulucu.SiteyiOnayla(authenticationMethod.EklentiTanımlayıcı, _siteContext.MevcutSite.Id) &&
                _eklentiBulucu.KullanıcıİçinYetkili(authenticationMethod.EklentiTanımlayıcı, _workContext.MevcutKullanıcı);
        }

        #endregion

        #region Authentication
        public virtual IActionResult Yetkilendir(HariciYetkilendirmeParametreleri parameters, string returnUrl = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (!HariciYetkilendirmeMetoduErişilebilir(parameters.ProviderSystemName))
                return ErrorAuthentication(new[] { "External authentication method cannot be loaded" }, returnUrl);

            //get current logged-in user
            var currentLoggedInUser = _workContext.MevcutKullanıcı.IsRegistered() ? _workContext.MevcutKullanıcı : null;

            //authenticate associated user if already exists
            var associatedUser = HariciYetkilendirmeParametresindenKullanıcıAl(parameters);
            if (associatedUser != null)
                return AuthenticateExistingUser(associatedUser, currentLoggedInUser, returnUrl);

            //or associate and authenticate new user
            return AuthenticateNewUser(currentLoggedInUser, parameters, returnUrl);
        }

        #endregion

        public virtual void HariciHesabıKullanıcıylaİlişkilendir(Kullanıcı kullanıcı, HariciYetkilendirmeParametreleri parameters)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException(nameof(kullanıcı));

            var hariciKimlikDoğrulamaKaydı = new HariciKimlikDoğrulamaKaydı
            {
                KullanıcıId = kullanıcı.Id,
                Email = parameters.Email,
                HariciTanımlayıcı = parameters.ExternalIdentifier,
                HariciGörünümTanımlayıcı = parameters.ExternalDisplayIdentifier,
                OAuthAccessToken = parameters.AccessToken,
                SağlayıcıSistemAdı = parameters.ProviderSystemName,
            };

            _hariciYetkilendirmeKaydıDepo.Ekle(hariciKimlikDoğrulamaKaydı);
        }
        public virtual Kullanıcı HariciYetkilendirmeParametresindenKullanıcıAl(HariciYetkilendirmeParametreleri parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = _hariciYetkilendirmeKaydıDepo.Tablo.FirstOrDefault(record =>
                record.HariciTanımlayıcı.Equals(parameters.ExternalIdentifier) && record.SağlayıcıSistemAdı.Equals(parameters.ProviderSystemName));
            if (associationRecord == null)
                return null;

            return _kullanıcıServisi.KullanıcıAlId(associationRecord.KullanıcıId);
        }
        public virtual void İlişkilendirmeyiKaldır(HariciYetkilendirmeParametreleri parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = _hariciYetkilendirmeKaydıDepo.Tablo.FirstOrDefault(record =>
                record.HariciTanımlayıcı.Equals(parameters.ExternalIdentifier) && record.SağlayıcıSistemAdı.Equals(parameters.ProviderSystemName));

            if (associationRecord != null)
                _hariciYetkilendirmeKaydıDepo.Sil(associationRecord);
        }
        public virtual void HariciYetkilendirmeKaydınıSil(HariciKimlikDoğrulamaKaydı hariciKimlikDoğrulamaKaydı)
        {
            if (hariciKimlikDoğrulamaKaydı == null)
                throw new ArgumentNullException(nameof(hariciKimlikDoğrulamaKaydı));

            _hariciYetkilendirmeKaydıDepo.Sil(hariciKimlikDoğrulamaKaydı);
        }

        #endregion
    }
}