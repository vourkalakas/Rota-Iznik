using System;
using System.Collections.Generic;
using Core;
using Core.Domain.Kullanıcılar;
using Core.Domain.Siparişler;

namespace Services.Kullanıcılar
{
    public partial interface IKullanıcıServisi
    {
        #region Kullanıcılar
        ISayfalıListe<Kullanıcı> TümKullanıcılarıAl(DateTime? tarihinden = null,
            DateTime? tarihine = null, int satıcıId = 0, int[] kullanıcıRolIdleri = null, 
            string email = null, string kullanıcıAdı = null,string ad = null, string soyadı = null,
            int doğumTarihi = 0, int doğumAyı = 0,string şirket = null, string tel = null,
            string postaKodu = null,string ipAdresi = null,int sayfaIndeksi = 0, 
            int sayfaBüyüklüğü = int.MaxValue);

        ISayfalıListe<Kullanıcı> OnlineKullanıcılarıAl(DateTime SonİşlemTarihi,
            int[] kullanıcıRolIdleri, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        void KullanıcıSil(Kullanıcı kullanıcı);
        Kullanıcı KullanıcıAlId(int kullanıcıId);
        IList<Kullanıcı> KullanıcıAlIdlerle(int[] kullanıcıIdleri);
        Kullanıcı KullanıcıAlGuid(Guid kullanıcıGuid);
        Kullanıcı KullanıcıAlEmail(string email);
        Kullanıcı KullanıcıAlSistemAdı(string sistemAdı);
        Kullanıcı KullanıcıAlKullanıcıAdı(string kullanıcıAdı);
        Kullanıcı ZiyaretciKullanıcıEkle();
        void KullanıcıEkle(Kullanıcı kullanıcı);
        void KullanıcıGüncelle(Kullanıcı kullanıcı);
        void ÖdemeVerileriniSıfırla(Kullanıcı kullanıcı, int siteId,
            bool kuponKodlarınıTemizle = false, bool ödemeÖznitelikleriniTemizle = false,
            bool ödülPuanlarınıTemizle = true,bool ödemeMetodunuTemizle = true);
        int ZiyaretciKullanıcıSil(DateTime? tarihinden, DateTime? tarihine, bool sepetiDoluOlanlarHariç);

        #endregion

        #region Kullanıcı rolleri

        void KullanıcıRolüSil(KullanıcıRolü kullanıcıRolü);
        KullanıcıRolü KullanıcıRolüAlId(int kullanıcıRolüId);
        KullanıcıRolü KullanıcıRolüAlSistemAdı(string sistemAdı);
        IList<KullanıcıRolü> TümKullanıcıRolleriniAl(bool gizliGöster = false);
        void KullanıcıRolüEkle(KullanıcıRolü kullanıcıRolü);
        void KullanıcıRolüGüncelle(KullanıcıRolü kullanıcıRolü);

        #endregion

        #region Kullanıcı şifreleri
        IList<KullanıcıŞifre> KullanıcıŞifreleriAl(int? kullanıcıId = null,
            ŞifreFormatı? şifreFormatı = null, int? geriDönenŞifreler = null);
        KullanıcıŞifre MevcutŞifreAl(int kullanıcıId);
        void KullanıcıŞifresiEkle(KullanıcıŞifre kullanıcıŞifresi);
        void KullanıcıŞifresiGüncelle(KullanıcıŞifre kullanıcıŞifresi);

        #endregion
    }
}
