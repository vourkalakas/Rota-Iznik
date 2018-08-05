using Microsoft.AspNetCore.Mvc;

namespace Web.Framework.Uzantılar
{
    public static class UrlYardımcısıUzantıları
    {
        public static string Giriş(this IUrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Giriş", "Kullanıcı", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Giriş", "Kullanıcı");
        }
        public static string Çıkış(this IUrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Çıkış", "Kullanıcı", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Çıkış", "Kullanıcı");
        }
    }
}
