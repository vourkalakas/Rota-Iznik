using Microsoft.AspNetCore.Http;

namespace Core
{

    public partial interface IWebYardımcısı
    {
        string UrlYönlendiriciAl();
        string MevcutIpAdresiAl();
        //string SayfanınUrlsiniAl(bool sorguİçerir);
        string SayfanınUrlsiniAl(bool sorguİçerir, bool? SslKullan=null, bool lowercaseUrl = false);
        bool MevcutBağlantıGüvenli();
        string SiteHostAl(bool SslKullan);
        string SiteKonumuAl(bool? SslKullan = null);
        bool SabitKaynak();
        string SorguDeğiştir(string url, string sorguDeğiştirme, string anchor);
        string SorguSil(string url, string sorgu);
        T Sorgu<T>(string ad);
        void AppDomainYenidenBaşlat(bool yönlendir = false);
        bool İstekYönlendirildi { get; }
        bool PostTamamlandı { get; set; }
        bool LocalIstek(HttpRequest req);
        string HamUrlAl(HttpRequest request);

    }
}
