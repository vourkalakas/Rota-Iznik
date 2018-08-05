using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Core.Domain.Kullanıcılar;
using Services.Kullanıcılar;

namespace Services.KimlikDoğrulama
{
    public partial class ÇerezKimlikDoğrulamaServisi : IKimlikDoğrulamaServisi
    {
        #region Fields

        private readonly KullanıcıAyarları _kullanıcıAyarları;
        private readonly IKullanıcıServisi _kullanıcıServisi;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Kullanıcı _önebellekKullanıcı;

        #endregion

        #region Ctor
        
        public ÇerezKimlikDoğrulamaServisi(KullanıcıAyarları kullanıcıAyarları,
            IKullanıcıServisi kullanıcıServisi,
            IHttpContextAccessor httpContextAccessor)
        {
            this._kullanıcıAyarları = kullanıcıAyarları;
            this._kullanıcıServisi = kullanıcıServisi;
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods
        public virtual async void Giriş(Kullanıcı kullanıcı, bool isPersistent)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException(nameof(kullanıcı));
           
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(kullanıcı.KullanıcıAdı))
                claims.Add(new Claim(ClaimTypes.Name, kullanıcı.KullanıcıAdı, ClaimValueTypes.String, ÇerezYetkilendirmeVarsayılanları.ClaimsIssuer));

            if (!string.IsNullOrEmpty(kullanıcı.Email))
                claims.Add(new Claim(ClaimTypes.Email, kullanıcı.Email, ClaimValueTypes.Email, ÇerezYetkilendirmeVarsayılanları.ClaimsIssuer));
            
            var userIdentity = new ClaimsIdentity(claims, ÇerezYetkilendirmeVarsayılanları.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };
            await _httpContextAccessor.HttpContext.SignInAsync(ÇerezYetkilendirmeVarsayılanları.AuthenticationScheme, userPrincipal, authenticationProperties);
            _önebellekKullanıcı = kullanıcı;
        }
        public virtual async void Çıkış()
        {
            _önebellekKullanıcı = null;
            await _httpContextAccessor.HttpContext.SignOutAsync(ÇerezYetkilendirmeVarsayılanları.AuthenticationScheme);
        }

        public virtual Kullanıcı KimliğiDoğrulananKullanıcı()
        {
            if (_önebellekKullanıcı != null)
                return _önebellekKullanıcı;
            
            var authenticateResult = _httpContextAccessor.HttpContext.AuthenticateAsync(ÇerezYetkilendirmeVarsayılanları.AuthenticationScheme).Result;
            if (!authenticateResult.Succeeded)
                return null;

            Kullanıcı kullanıcı = null;
            if (_kullanıcıAyarları.KullanıcıAdlarıEtkin)
            {
                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                    && claim.Issuer.Equals(ÇerezYetkilendirmeVarsayılanları.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (usernameClaim != null)
                    kullanıcı = _kullanıcıServisi.KullanıcıAlKullanıcıAdı(usernameClaim.Value);
            }
            else
            {
                var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email 
                    && claim.Issuer.Equals(ÇerezYetkilendirmeVarsayılanları.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (emailClaim != null)
                    kullanıcı = _kullanıcıServisi.KullanıcıAlEmail(emailClaim.Value);
            }
            if (kullanıcı == null || !kullanıcı.Aktif || kullanıcı.GirişGerekli || kullanıcı.Silindi || !kullanıcı.IsRegistered())
                return null;
            
            _önebellekKullanıcı = kullanıcı;

            return _önebellekKullanıcı;
        }

        #endregion
    }
}