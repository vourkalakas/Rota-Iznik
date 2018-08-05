using System.Collections.Generic;
using Core.Domain.Kullanıcılar;
using Core.Domain.Güvenlik;


namespace Services.Güvenlik
{
    public partial class StandartİzinSağlayıcı : IİzinSağlayıcı
    {
        //admin area permissions
        public static readonly İzinKaydı YöneticiBölgesiErişimi = new İzinKaydı { Adı = "Yönetici alanı. Erişim", SistemAdı = "YöneticiBölgesiErişimi", Kategori = "Standart" };
        public static readonly İzinKaydı KullanıcıTaklidiİzinli = new İzinKaydı { Adı = "Yönetici alanı. Kullanıcı Taklidi İzinli", SistemAdı = "KullanıcıTaklidiİzinli", Kategori = "Kullanıcılar" };
        public static readonly İzinKaydı ÜrünleriYönet = new İzinKaydı { Adı = "Yönetici alanı. Ürünleri Yönet", SistemAdı = "ÜrünleriYönet", Kategori = "Katalog" };
        public static readonly İzinKaydı KategorileriYönet = new İzinKaydı { Adı = "Yönetici alanı. KategorileriYönet", SistemAdı = "KategorileriYönet", Kategori = "Katalog" };
        public static readonly İzinKaydı ÜrünİncelemeleriniYönet = new İzinKaydı { Adı = "Yönetici alanı. Ürün İncelemelerini Yönet", SistemAdı = "ÜrünİncelemeleriniYönet", Kategori = "Katalog" };
        public static readonly İzinKaydı ÜrünTaglarınıYönet = new İzinKaydı { Adı = "Yönetici alanı. Ürün Taglarını Yönet", SistemAdı = "ÜrünTaglarınıYönet", Kategori = "Katalog" };
        public static readonly İzinKaydı ÜrünÖznitelikleriYönet = new İzinKaydı { Adı = "Yönetici alanı. Ürün Öznitelikleri Yönet", SistemAdı = "ÜrünÖznitelikleriYönet", Kategori = "Katalog" };
       
        public static readonly İzinKaydı MevcutSepetiYönet = new İzinKaydı { Adı = "Yönetici alanı. Mevcut Sepeti Yönet", SistemAdı = "MevcutSepetiYönet", Kategori = "Siparişler" };
        public static readonly İzinKaydı SiparişleriYönet = new İzinKaydı { Adı = "Yönetici alanı. Siparişleri Yönet", SistemAdı = "SiparişleriYönet", Kategori = "Siparişler" };
        public static readonly İzinKaydı HediyeKartlarınıYönet = new İzinKaydı { Adı = "Yönetici alanı. Hediye Kartlarını Yönet", SistemAdı = "HediyeKartlarınıYönet", Kategori = "Siparişler" };
        public static readonly İzinKaydı İadeİstekleriniYönet = new İzinKaydı { Adı = "Yönetici alanı. İade İsteklerini Yönet", SistemAdı = "İadeİstekleriniYönet", Kategori = "Siparişler" };
        public static readonly İzinKaydı ÜlkeSiparişRaporErişimi = new İzinKaydı { Adı = "Yönetici alanı. Ülke Sipariş Rapor Erişimi", SistemAdı = "ÜlkeSiparişRaporErişimi", Kategori = "Siparişler" };
        public static readonly İzinKaydı KampanyalarıYönet = new İzinKaydı { Adı = "Yönetici alanı. Kampanyaları Yönet", SistemAdı = "KampanyalarıYönet", Kategori = "Promosyon" };
        public static readonly İzinKaydı İndirimleriYönet = new İzinKaydı { Adı = "Yönetici alanı. İndirimleri Yönet", SistemAdı = "İndirimleriYönet", Kategori = "Promosyon" };
        public static readonly İzinKaydı BültenAboneleriniYönet = new İzinKaydı { Adı = "Yönetici alanı. Bülten Abonelerini Yönet", SistemAdı = "BültenAboneleriniYönet", Kategori = "Promosyon" };
        public static readonly İzinKaydı AnketleriYönet = new İzinKaydı { Adı = "Yönetici alanı. Anketleri Yönet", SistemAdı = "AnketleriYönet", Kategori = "İçerik Yönetimi" };
        public static readonly İzinKaydı HaberleriYönet = new İzinKaydı { Adı = "Yönetici alanı. Haberleri Yönet", SistemAdı = "HaberleriYönet", Kategori = "İçerik Yönetimi" };
        public static readonly İzinKaydı BlogYönet = new İzinKaydı { Adı = "Yönetici alanı. Blog Yönet", SistemAdı = "BlogYönet", Kategori = "İçerik Yönetimi" };
        public static readonly İzinKaydı AraçlarıYönet = new İzinKaydı { Adı = "Yönetici alanı. Araçları Yönet", SistemAdı = "AraçlarıYönet", Kategori = "İçerik Yönetimi" };
        public static readonly İzinKaydı SayfalarıYönet = new İzinKaydı { Adı = "Yönetici alanı. Sayfaları Yönet", SistemAdı = "SayfalarıYönet", Kategori = "İçerik Yönetimi" };
        public static readonly İzinKaydı ForumYönet = new İzinKaydı { Adı = "Yönetici alanı. Forum Yönet", SistemAdı = "ForumYönet", Kategori = "İçerik Yönetimi" };
        public static readonly İzinKaydı MesajTemalarınıYönet = new İzinKaydı { Adı = "Yönetici alanı. Mesaj Temalarını Yönet", SistemAdı = "MesajTemalarınıYönet", Kategori = "İçerik Yönetimi" };
        public static readonly İzinKaydı ÜlkeleriYönet = new İzinKaydı { Adı = "Yönetici alanı. Ülkeleri Yönet", SistemAdı = "ÜlkeleriYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı ÖdemeMetodlarıYönet = new İzinKaydı { Adı = "Yönetici alanı. Ödeme Metodları Yönet", SistemAdı = "ÖdemeMetodlarıYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı HariciGirişMetodlarınıYönet = new İzinKaydı { Adı = "Yönetici alanı. Harici Giriş Metodlarını Yönet", SistemAdı = "HariciGirişMetodlarınıYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı VergiAyarlarınıYönet = new İzinKaydı { Adı = "Yönetici alanı. Vergi Ayarlarını Yönet", SistemAdı = "VergiAyarlarınıYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı DövizYönet = new İzinKaydı { Adı = "Yönetici alanı. Döviz Yönet", SistemAdı = "DövizYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı İşlemGeçmişiYönet = new İzinKaydı { Adı = "Yönetici alanı. İşlem Geçmişi Yönet", SistemAdı = "İşlemGeçmişiYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı EmailHesaplarıYönet = new İzinKaydı { Adı = "Yönetici alanı. Email Hesapları Yönet", SistemAdı = "EmailHesaplarıYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı SiteleriYönet = new İzinKaydı { Adı = "Yönetici alanı. Siteleri Yönet", SistemAdı = "SiteleriYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı EklentileriYönet = new İzinKaydı { Adı = "Yönetici alanı. Eklentileri Yönet", SistemAdı = "EklentileriYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı SistemLogYönet = new İzinKaydı { Adı = "Yönetici alanı. Sistem Log Yönet", SistemAdı = "SistemLogYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı SıralanmışMesajlarıYönet = new İzinKaydı { Adı = "Yönetici alanı. Sıralanmış Mesajları Yönet", SistemAdı = "SıralanmışMesajlarıYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı BakımYönet = new İzinKaydı { Adı = "Yönetici alanı. Bakım Yönet", SistemAdı = "BakımYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı HTMLEditörResimleriYönet = new İzinKaydı { Adı = "Yönetici alanı. HTML Editör. Resimleri Yönet", SistemAdı = "HtmlEditor.ManagePictures", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı ZamanlananGörevleriniYönetin = new İzinKaydı { Adı = "Yönetici alanı. Zamanlanan Görevlerini Yönetin", SistemAdı = "ZamanlananGörevleriniYönetin", Kategori = "Yapılandırma" };


        //public store permissions
        public static readonly İzinKaydı FiyatlarıGörüntüle = new İzinKaydı { Adı = "Site. Fiyatları Görüntüle", SistemAdı = "FiyatlarıGörüntüle", Kategori = "Site" };
        public static readonly İzinKaydı SepetEtkin = new İzinKaydı { Adı = "Site. Sepet Etkin", SistemAdı = "SepetEtkin", Kategori = "Site" };
        public static readonly İzinKaydı Navigasyonİzinli = new İzinKaydı { Adı = "Site. Navigasyon İzinli", SistemAdı = "Navigasyonİzinli", Kategori = "Site" };
        public static readonly İzinKaydı KapalıSiteyeErişim = new İzinKaydı { Adı = "Site. Kapalı Siteye Erişim", SistemAdı = "KapalıSiteyeErişim", Kategori = "Site" };
        public static readonly İzinKaydı BankaYönet = new İzinKaydı { Adı = "Site. Banka Yönet", SistemAdı = "BankaYönet", Kategori = "Site" };
        public static readonly İzinKaydı MusteriYönet = new İzinKaydı { Adı = "Site. Musteri Yönet", SistemAdı = "MusteriYönet", Kategori = "Site" };
        public static readonly İzinKaydı TedarikciYönet = new İzinKaydı { Adı = "Site. Tedarikci Yönet", SistemAdı = "TedarikciYönet", Kategori = "Site" };
        public static readonly İzinKaydı HariciYönet = new İzinKaydı { Adı = "Site. Harici Yönet", SistemAdı = "HariciYönet", Kategori = "Site" };
        public static readonly İzinKaydı TeklifKalemiYönet = new İzinKaydı { Adı = "Site. Teklif Kalemi Yönet", SistemAdı = "TeklifKalemiYönet", Kategori = "Site" };
        public static readonly İzinKaydı UnvanYönet = new İzinKaydı { Adı = "Site. Unvan Yönet", SistemAdı = "UnvanYönet", Kategori = "Site" };
        public static readonly İzinKaydı OtelYönet = new İzinKaydı { Adı = "Site. Otel Yönet", SistemAdı = "OtelYönet", Kategori = "Site" };
        public static readonly İzinKaydı YDAcenteYönet = new İzinKaydı { Adı = "Site. YDAcente Yönet", SistemAdı = "YDAcenteYönet", Kategori = "Site" };
        public static readonly İzinKaydı TeklifYönet = new İzinKaydı { Adı = "Site. Teklif Yönet", SistemAdı = "TeklifYönet", Kategori = "Site" };
        public static readonly İzinKaydı TeklifHariciYönet = new İzinKaydı { Adı = "Site. Teklif Harici Yönet", SistemAdı = "TeklifHariciYönet", Kategori = "Site" };
        public static readonly İzinKaydı YetkiliYönet = new İzinKaydı { Adı = "Site. Yetkili Yönet", SistemAdı = "YetkiliYönet", Kategori = "Site" };
        public static readonly İzinKaydı GorusmeRaporlariYönet = new İzinKaydı { Adı = "Site. Gorusme Raporlari Yönet", SistemAdı = "GorusmeRaporlariYönet", Kategori = "Site" };
        public static readonly İzinKaydı OdemeFormuYönet = new İzinKaydı { Adı = "Site. Odeme Formu Yönet", SistemAdı = "OdemeFormuYönet", Kategori = "Site" };
        public static readonly İzinKaydı KullanıcılarıYönet = new İzinKaydı { Adı = "Site. Kullanıcıları Yönet", SistemAdı = "KullanıcılarıYönet", Kategori = "Kullanıcılar" };
        public static readonly İzinKaydı KonaklamaYönet = new İzinKaydı { Adı = "Site. Konaklama Yönet", SistemAdı = "KonaklamaYönet", Kategori = "Site" };
        public static readonly İzinKaydı KursYönet = new İzinKaydı { Adı = "Site. Kurs Yönet", SistemAdı = "KursYönet", Kategori = "Site" };
        public static readonly İzinKaydı KayıtYönet = new İzinKaydı { Adı = "Site. Kayıt Yönet", SistemAdı = "KayıtYönet", Kategori = "Site" };
        public static readonly İzinKaydı TransferYönet = new İzinKaydı { Adı = "Site. Transfer Yönet", SistemAdı = "TransferYönet", Kategori = "Site" };
        public static readonly İzinKaydı KatılımcıYönet = new İzinKaydı { Adı = "Site. Katılımcı Yönet", SistemAdı = "KatılımcıYönet", Kategori = "Site" };
        public static readonly İzinKaydı OdemeFormuGoruntule = new İzinKaydı { Adı = "Site. Odeme Formu Goruntule", SistemAdı = "OdemeFormuGoruntule", Kategori = "Site" };
        public static readonly İzinKaydı AyarlarıYönet = new İzinKaydı { Adı = "Site. Ayarları Yönet", SistemAdı = "AyarlarıYönet", Kategori = "Yapılandırma" };
        public static readonly İzinKaydı ACLYönet = new İzinKaydı { Adı = "Site. ACL Yönet", SistemAdı = "ACLYönet", Kategori = "Yapılandırma" };
        public virtual IEnumerable<İzinKaydı> İzinleriAl()
        {
            return new[]
            {
                YöneticiBölgesiErişimi,
                KullanıcıTaklidiİzinli,
                ÜrünleriYönet,
                KategorileriYönet,
                ÜrünİncelemeleriniYönet,
                ÜrünTaglarınıYönet,
                ÜrünÖznitelikleriYönet,
                KullanıcılarıYönet,
                MevcutSepetiYönet,
                SiparişleriYönet,
                HediyeKartlarınıYönet,
                İadeİstekleriniYönet,
                ÜlkeSiparişRaporErişimi,
                KampanyalarıYönet,
                İndirimleriYönet,
                BültenAboneleriniYönet,
                AnketleriYönet,
                HaberleriYönet,
                BlogYönet,
                AraçlarıYönet,
                SayfalarıYönet,
                ForumYönet,
                MesajTemalarınıYönet,
                ÜlkeleriYönet,
                AyarlarıYönet,
                ÖdemeMetodlarıYönet,
                HariciGirişMetodlarınıYönet,
                VergiAyarlarınıYönet,
                DövizYönet,
                İşlemGeçmişiYönet,
                ACLYönet,
                EmailHesaplarıYönet,
                SiteleriYönet,
                EklentileriYönet,
                SistemLogYönet,
                SıralanmışMesajlarıYönet,
                BakımYönet,
                HTMLEditörResimleriYönet,
                ZamanlananGörevleriniYönetin,
                FiyatlarıGörüntüle,
                SepetEtkin,
                Navigasyonİzinli,
                KapalıSiteyeErişim
            };
        }

        public virtual IEnumerable<VarsayılanİzinKaydı> VarsayılanİzinleriAl()
        {
            return new[]
            {
                new VarsayılanİzinKaydı
                {
                    KullanıcıRolüSistemAdı = SistemKullanıcıRolAdları.Yönetici,
                    İzinKayıtları = new[]
                    {
                        YöneticiBölgesiErişimi,
                        KullanıcıTaklidiİzinli,
                        ÜrünleriYönet,
                        KategorileriYönet,
                        ÜrünİncelemeleriniYönet,
                        ÜrünTaglarınıYönet,
                        ÜrünÖznitelikleriYönet,
                        KullanıcılarıYönet,
                        MevcutSepetiYönet,
                        SiparişleriYönet,
                        HediyeKartlarınıYönet,
                        İadeİstekleriniYönet,
                        ÜlkeSiparişRaporErişimi,
                        KampanyalarıYönet,
                        İndirimleriYönet,
                        BültenAboneleriniYönet,
                        AnketleriYönet,
                        HaberleriYönet,
                        BlogYönet,
                        AraçlarıYönet,
                        SayfalarıYönet,
                        ForumYönet,
                        MesajTemalarınıYönet,
                        ÜlkeleriYönet,
                        AyarlarıYönet,
                        ÖdemeMetodlarıYönet,
                        HariciGirişMetodlarınıYönet,
                        VergiAyarlarınıYönet,
                        DövizYönet,
                        İşlemGeçmişiYönet,
                        ACLYönet,
                        EmailHesaplarıYönet,
                        SiteleriYönet,
                        EklentileriYönet,
                        SistemLogYönet,
                        SıralanmışMesajlarıYönet,
                        BakımYönet,
                        HTMLEditörResimleriYönet,
                        ZamanlananGörevleriniYönetin,
                        FiyatlarıGörüntüle,
                        SepetEtkin,
                        Navigasyonİzinli,
                        KapalıSiteyeErişim
                    }
                },
                new VarsayılanİzinKaydı
                {
                    KullanıcıRolüSistemAdı = SistemKullanıcıRolAdları.Ziyaretçi,
                    İzinKayıtları = new[]
                    {
                        FiyatlarıGörüntüle,
                        SepetEtkin,
                        Navigasyonİzinli
                    }
                },
                new VarsayılanİzinKaydı
                {
                    KullanıcıRolüSistemAdı = SistemKullanıcıRolAdları.Kayıtlı,
                    İzinKayıtları = new[]
                    {
                        FiyatlarıGörüntüle,
                        SepetEtkin,
                        Navigasyonİzinli
                    }
                },
            };
        }
    }
}
