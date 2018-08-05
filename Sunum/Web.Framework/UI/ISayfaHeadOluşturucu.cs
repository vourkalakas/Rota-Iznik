using Microsoft.AspNetCore.Mvc;

namespace Web.Framework.UI
{
    public partial interface ISayfaHeadOluşturucu
    {
        void BaşlıkParçasıEkle(string parça);
        void BaşlıkParçasıIlaveEt(string parça);
        string BaşlıkOluştur(bool addDefaultTitle);
        void MetaDescriptionParçasıEkle(string parça);
        void MetaDescriptionParçasıIlaveEt(string parça);
        string MetaDescriptionOluştur();
        void MetaKeywordParçasıEkle(string parça);
        void MetaKeywordParçasıIlaveEt(string parça);
        string MetaKeywordsOluştur();
        void ScriptParçasıEkle(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında, bool isAync);
        void ScriptParçasıIlaveEt(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında, bool isAsync);
        string ScriptsOluştur(IUrlHelper urlHelper, KaynakKonumu konum, bool? paketDosyaları = null);
        void InlineScriptParçasıEkle(KaynakKonumu konum, string script);
        void InlineScriptParçasıIlaveEt(KaynakKonumu konum, string script);
        string InlineScriptsOluştur(IUrlHelper urlHelper, KaynakKonumu konum);
        void CssParçasıEkle(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında = false);
        void CssParçasıIlaveEt(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında = false);
        string CssOluştur(IUrlHelper urlHelper, KaynakKonumu konum, bool? paketDosyaları = null);
        void CanonicalUrlParçasıEkle(string parça);
        void CanonicalUrlParçasıIlaveEt(string parça);
        string CanonicalUrlOluştur();
        void ÖzelHeadParçasıEkle(string parça);
        void ÖzelHeadParçasıIlaveEt(string parça);
        string ÖzelHeadOluştur();
        void SayfaCssSınıfıParçasıEkle(string parça);
        void SayfaCssSınıfıParçasıIlaveEt(string parça);
        string SayfaCssSınıfıOluştur();
        void DüzenleSayfaURLsiEkle(string url);
        string DüzenleSayfaURLsiAl();
        void AktifMenuÖğesiSistemAdıBelirle(string systemName);
        string AktifMenuÖğesiSistemAdıAl();
    }
}