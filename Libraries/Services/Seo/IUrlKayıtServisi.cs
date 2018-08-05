using Core;
using Core.Domain.Seo;
using System.Collections.Generic;

namespace Services.Seo
{
    public partial interface IUrlKayıtServisi
    {
        void UrlKaydınıSil(UrlKaydı urlKaydı);

        void UrlKayıtlarınıSil(IList<UrlKaydı> urlKayıtları);

        UrlKaydı UrlKaydıAlId(int urlKayıtIdleri);

        IList<UrlKaydı> UrlKayıtlarınıAlId(int[] urlKayıtIdleri);

        void UrlKaydıEkle(UrlKaydı urlKaydı);

        void UrlKaydıGüncelle(UrlKaydı urlKaydı);

        UrlKaydı SlugİleAl(string slug);

        UrlKayıtServisi.ÖnbellekİçinUrlKaydı SlugİleAlÖnbelleklenmiş(string slug);

        ISayfalıListe<UrlKaydı> TümUrlKayıtlarınıAl(string slug = "", int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);

        string AktifSlugAl(int varlıkId, string varlıkAdı);

        void SlugKaydet<T>(T varlık, string slug) where T : TemelVarlık, ISlugDestekli;
    }
}
