using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Core.Altyapı;
using Core.Domain.Seo;


namespace Web.Framework.UI
{
    public static class ArayüzEklentileri
    {
        public static void BaşlıkParçasıEkle(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.BaşlıkParçasıEkle(parça);
        }
        public static void BaşlıkParçasıIlaveEt(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.BaşlıkParçasıIlaveEt(parça);
        }
        public static IHtmlContent Başlık(this IHtmlHelper html, bool VarsayılanBaşlıkEkle = true, string parça = "")
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            html.BaşlıkParçasıIlaveEt(parça);
            return new HtmlString(html.Encode(sayfaHeadOluşturucu.BaşlıkOluştur(VarsayılanBaşlıkEkle)));
        }
        public static void MetaDescriptionParçasıEkle(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.MetaDescriptionParçasıEkle(parça);
        }
        public static void MetaDescriptionParçasıIlaveEt(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.MetaDescriptionParçasıIlaveEt(parça);
        }
        public static IHtmlContent MetaDescription(this IHtmlHelper html, string parça = "")
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            html.MetaDescriptionParçasıIlaveEt(parça);
            return new HtmlString(html.Encode(sayfaHeadOluşturucu.MetaDescriptionOluştur()));
        }
        public static void MetaKeywordParçasıEkle(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.MetaKeywordParçasıEkle(parça);
        }
        public static void MetaKeywordParçasıIlaveEt(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.MetaKeywordParçasıIlaveEt(parça);
        }
        public static IHtmlContent MetaKeywords(this IHtmlHelper html, string parça = "")
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            html.MetaKeywordParçasıIlaveEt(parça);
            return new HtmlString(html.Encode(sayfaHeadOluşturucu.MetaKeywordsOluştur()));
        }
        public static void ScriptParçasıEkle(this IHtmlHelper html, string parça, string debugSrc = "", bool pakettenÇıkar = false, bool async = false)
        {
            ScriptParçasıEkle(html, KaynakKonumu.Head, parça, debugSrc, pakettenÇıkar, async);
        }
        public static void ScriptParçasıEkle(this IHtmlHelper html, KaynakKonumu konum, string parça, string debugSrc = "", bool pakettenÇıkar = false, bool async = false)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.ScriptParçasıEkle(konum, parça, debugSrc, pakettenÇıkar, async);
        }
        public static void ScriptParçasıIlaveEt(this IHtmlHelper html, string parça, string debugSrc = "", bool pakettenÇıkar = false, bool async = false)
        {
            ScriptParçasıIlaveEt(html, KaynakKonumu.Head, parça, debugSrc, pakettenÇıkar, async);
        }
        public static void ScriptParçasıIlaveEt(this IHtmlHelper html, KaynakKonumu konum, string parça, string debugSrc = "", bool pakettenÇıkar = false, bool async = false)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.ScriptParçasıIlaveEt(konum, parça, debugSrc, pakettenÇıkar, async);
        }
        public static IHtmlContent Scripts(this IHtmlHelper html, IUrlHelper urlHelper,
            KaynakKonumu konum, bool? paketDosyaları = null)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            return new HtmlString(sayfaHeadOluşturucu.ScriptsOluştur(urlHelper, konum, paketDosyaları));
        }
        public static void CssParçasıEkle(this IHtmlHelper html, string parça, string debugSrc = "", bool pakettenÇıkar = false)
        {
            CssParçasıEkle(html, KaynakKonumu.Head, parça, debugSrc, pakettenÇıkar);
        }
        public static void CssParçasıEkle(this IHtmlHelper html, KaynakKonumu konum, string parça, string debugSrc = "", bool pakettenÇıkar = false)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.CssParçasıEkle(konum, parça, debugSrc, pakettenÇıkar);
        }
        public static void CssParçasıIlaveEt(this IHtmlHelper html, string parça, string debugSrc = "", bool pakettenÇıkar = false)
        {
            CssParçasıIlaveEt(html, KaynakKonumu.Head, parça, debugSrc, pakettenÇıkar);
        }
        public static void CssParçasıIlaveEt(this IHtmlHelper html, KaynakKonumu konum, string parça, string debugSrc = "", bool pakettenÇıkar = false)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.CssParçasıIlaveEt(konum, parça, debugSrc, pakettenÇıkar);
        }
        public static IHtmlContent CssFiles(this IHtmlHelper html, IUrlHelper urlHelper,
            KaynakKonumu konum, bool? paketDosyaları = null)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            return new HtmlString(sayfaHeadOluşturucu.CssOluştur(urlHelper, konum, paketDosyaları));
        }
        public static void CanonicalUrlParçasıEkle(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.CanonicalUrlParçasıEkle(parça);
        }
        public static void CanonicalUrlParçasıIlaveEt(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.CanonicalUrlParçasıIlaveEt(parça);
        }
        public static IHtmlContent CanonicalUrls(this IHtmlHelper html, string parça = "")
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            html.CanonicalUrlParçasıIlaveEt(parça);
            return new HtmlString(sayfaHeadOluşturucu.CanonicalUrlOluştur());
        }
        public static void ÖzelHeadParçasıEkle(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.ÖzelHeadParçasıEkle(parça);
        }
        public static void ÖzelHeadParçasıIlaveEt(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.ÖzelHeadParçasıIlaveEt(parça);
        }
        public static IHtmlContent ÖzelHead(this IHtmlHelper html)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            return new HtmlString(sayfaHeadOluşturucu.ÖzelHeadOluştur());
        }
        public static void SayfaCssSınıfıParçasıEkle(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.SayfaCssSınıfıParçasıEkle(parça);
        }
        public static void SayfaCssSınıfıParçasıIlaveEt(this IHtmlHelper html, string parça)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.SayfaCssSınıfıParçasıIlaveEt(parça);
        }
        public static IHtmlContent SayfaCssSınıfı(this IHtmlHelper html, string parça = "", bool sınıfElemanıDahilEt = true)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            html.SayfaCssSınıfıParçasıIlaveEt(parça);
            var sınıflar = sayfaHeadOluşturucu.SayfaCssSınıfıOluştur();

            if (string.IsNullOrEmpty(sınıflar))
                return null;

            var sonuç = sınıfElemanıDahilEt ? string.Format("class=\"{0}\"", sınıflar) : sınıflar;
            return new HtmlString(sonuç);
        }
        public static void AktifMenuÖğesiSistemAdıBelirle(this IHtmlHelper html, string sistemAdı)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            sayfaHeadOluşturucu.AktifMenuÖğesiSistemAdıBelirle(sistemAdı);
        }
        public static string AktifMenuÖğesiSistemAdıAl(this IHtmlHelper html)
        {
            var sayfaHeadOluşturucu = EngineContext.Current.Resolve<ISayfaHeadOluşturucu>();
            return sayfaHeadOluşturucu.AktifMenuÖğesiSistemAdıAl();
        }
    }
}

