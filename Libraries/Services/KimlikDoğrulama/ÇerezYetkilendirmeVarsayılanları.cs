using Microsoft.AspNetCore.Http;

namespace Services.KimlikDoğrulama
{
    public static class ÇerezYetkilendirmeVarsayılanları
    {
        public const string AuthenticationScheme = "Yetkilendirme";
        public const string ExternalAuthenticationScheme = "HariciYetkilendirme";
        public static readonly string CookiePrefix = ".Rota.";
        public static readonly string ClaimsIssuer = "RotaIznik";
        public static readonly PathString LoginPath = new PathString("/giriş");
        public static readonly PathString LogoutPath = new PathString("/çıkış");
        public static readonly PathString AccessDeniedPath = new PathString("/sayfa-bulunamadı");
        public static readonly string ReturnUrlParameter = "";
    }
}
