using Core;
using Core.Domain.Forum;
using Core.Domain.Kullanıcılar;
using System;
using System.Collections.Generic;

namespace Services.Forumlar
{
    public partial interface IForumServisi
    {
        void ForumGrubunuSil(ForumGrubu forumGrubu);
        ForumGrubu ForumGrubuAlId(int forumGrupId);
        IList<ForumGrubu> TümForumGruplarınıAl();
        void ForumGrubuEkle(ForumGrubu forumGrubu);
        void ForumGrubuGüncelle(ForumGrubu forumGrubu);
        void ForumSil(Forum forum);
        Forum ForumAlId(int forumId);
        IList<Forum> TümForumlarıAlGrupId(int forumGrupId);
        void ForumEkle(Forum forum);
        void ForumGüncelle(Forum forum);
        void SayfaSil(ForumSayfası forumSayfası);
        ForumSayfası SayfaAlId(int forumSayfasıId);
        ForumSayfası SayfaAlId(int forumSayfasıId, bool görüntülemeleriArtır);
        ISayfalıListe<ForumSayfası> TümSayfalarıAl(int forumId = 0,
            int kullanıcıId = 0, string anahtarKelimeler = "", ForumAramaTipi aramaTipi = ForumAramaTipi.Tümü,
            int günSınırı = 0, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        ISayfalıListe<ForumSayfası> AktifSayfalarıAl(int forumId = 0,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        void SayfaEkle(ForumSayfası forumSayfası, bool bildirimGönder);
        void SayfaGüncelle(ForumSayfası forumSayfası);
        ForumSayfası SayfayıTaşı(int forumSayfasıId, int yeniForumId);
        void GirdiSil(ForumGirdisi forumGirdisi);
        ForumGirdisi GirdiAlId(int forumGirdisiId);
        ISayfalıListe<ForumGirdisi> TümGirdileriAl(int forumSayfasıId = 0,
            int kullanıcıId = 0, string anahtarKelimeler = "",
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        ISayfalıListe<ForumGirdisi> TümGirdileriAl(int forumSayfasıId = 0, int kullanıcıId = 0,
            string anahtarKelimeler = "", bool azalanSırala = false,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        void GirdiEkle(ForumGirdisi forumGirdisi, bool bildirimGönder);
        void GirdiGüncelle(ForumGirdisi forumGirdisi);
        void ÖzelMesajSil(ÖzelMesaj özelMesaj);
        ÖzelMesaj ÖzelMesajAlId(int özelMesajId);
        ISayfalıListe<ÖzelMesaj> TümÖzelMesajlarıAl(int siteId, int kulanıcıdanId,
            int kullanıcıyaId, bool? okundu, bool? yazarTarafındanSilindi, bool? alıcıTarafındanSilindi,
            string anahtarKelimeler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        void ÖzelMesajEkle(ÖzelMesaj özelMesaj);
        void ÖzelMesajGüncelle(ÖzelMesaj özelMesaj);
        void AbonelikSil(ForumAboneliği forumAboneliği);
        ForumAboneliği AbonelikAlId(int forumAboneliğiId);
        ISayfalıListe<ForumAboneliği> TümAbonelikleriAl(int kullanıcıId = 0, int forumId = 0,
            int sayfaId = 0, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        void AbonelikEkle(ForumAboneliği forumAboneliği);
        void AbonelikGüncelle(ForumAboneliği forumAboneliği);
        bool KullanıcıSayfaOluşturabilir(Kullanıcı kullanıcı, Forum forum);
        bool KullanıcıSayfaDüzenleyebilir(Kullanıcı kullanıcı, ForumSayfası topic);
        bool KullanıcıSayfaTaşıyabilir(Kullanıcı kullanıcı, ForumSayfası topic);
        bool KullanıcıSayfaSilebilir(Kullanıcı kullanıcı, ForumSayfası topic);
        bool KullanıcıSayfaGirdisiOluşturabilir(Kullanıcı kullanıcı, ForumSayfası topic);
        bool KullanıcıSayfaGirdisiDüzenleyebilir(Kullanıcı kullanıcı, ForumGirdisi post);
        bool KullanıcıSayfaGirdisiSilebilir(Kullanıcı kullanıcı, ForumGirdisi post);
        bool KullanıcıSayfaÖnceliğiDüzenleyebilir(Kullanıcı kullanıcı);
        bool KullanıcıAboneOlabilir(Kullanıcı kullanıcı);
        int SayfaIndeksiHesapla(int forumSayfasıId, int sayfaBüyüklüğü, int postId);
        ForumGirdisiOyu ForumGirdisiOyuAl(int girdiId, Kullanıcı kullanıcı);
        int ForumGirdisiOyuSayısıAl(Kullanıcı kullanıcı, DateTime oluşturulmaTarihi);
        void ForumGirdisiOyuEkle(ForumGirdisiOyu girdiOyu);
        void ForumGirdisiOyuGüncelle(ForumGirdisiOyu girdiOyu);
        void ForumGirdisiOyuSil(ForumGirdisiOyu girdiOyu);
    }
}

