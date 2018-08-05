using System;
using System.Collections.Generic;
using Core;
using Core.Domain.Forum;
using Core.Domain.Kullanıcılar;
using Core.Data;
using Core.Önbellek;
using Services.Genel;
using Services.Kullanıcılar;
using Services.Mesajlar;
using Services.Olaylar;
using System.Linq;

namespace Services.Forumlar
{
    public partial class ForumServisi : IForumServisi
    {
        private const string FORUMGRUP_ALL_KEY = "forumgrup.all";
        private const string FORUM_ALLBYFORUMGROUPID_KEY = "forum.allbyforumgrupid-{0}";
        private const string FORUMGRUP_PATTERN_KEY = "forumgrup.";
        private const string FORUM_PATTERN_KEY = "forum.";
        private readonly IDepo<ForumGrubu> _forumGrupDepo;
        private readonly IDepo<Forum> _forumDepo;
        private readonly IDepo<ForumSayfası> _forumSayfaDepo;
        private readonly IDepo<ForumGirdisi> _forumGirdisiDepo;
        private readonly IDepo<ForumGirdisiOyu> _forumGirdiOyuDepo;
        private readonly IDepo<ÖzelMesaj> _forumÖzelMesajDepo;
        private readonly IDepo<ForumAboneliği> _forumAboneliğiDepo;
        private readonly ForumAyarları _forumAyarları;
        private readonly IDepo<Kullanıcı> _kullanıcıDepo;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IGenelÖznitelikServisi _genelÖznitelikServisi;
        private readonly IKullanıcıServisi _kullanıcıServisi;
        private readonly IWorkContext _workContext;
        private readonly IMesajServisi _mesajServisi;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        public ForumServisi(
            IDepo<ForumGrubu> forumGrupDepo,
            IDepo<Forum> forumDepo,
            IDepo<ForumSayfası> forumSayfaDepo,
            IDepo<ForumGirdisi> forumGirdisiDepo,
            IDepo<ForumGirdisiOyu> forumGirdiOyuDepo,
            IDepo<ÖzelMesaj> forumÖzelMesajDepo,
            IDepo<ForumAboneliği> forumAboneliğiDepo,
            ForumAyarları _forumAyarları,
            IDepo<Kullanıcı> kullanıcıDepo,
            IÖnbellekYönetici önbellekYönetici,
            IGenelÖznitelikServisi genelÖznitelikServisi,
            IKullanıcıServisi kullanıcıServisi,
            IWorkContext workContext,
            IMesajServisi mesajServisi,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._forumGrupDepo = forumGrupDepo;
            this._forumDepo = forumDepo;
            this._forumSayfaDepo = forumSayfaDepo;
            this._forumGirdisiDepo = forumGirdisiDepo;
            this._forumÖzelMesajDepo = forumÖzelMesajDepo;
            this._forumAboneliğiDepo = forumAboneliğiDepo;
            this._forumAyarları = _forumAyarları;
            this._kullanıcıDepo = kullanıcıDepo;
            this._önbellekYönetici = önbellekYönetici;
            this._genelÖznitelikServisi = genelÖznitelikServisi;
            this._kullanıcıServisi = kullanıcıServisi;
            this._workContext = workContext;
            this._mesajServisi = mesajServisi;
            this._olayYayınlayıcı = olayYayınlayıcı;

        }

        private bool Kayıtlı(Kullanıcı kullanıcı)
        {
            if (kullanıcı.IsRegistered())
            {
                return true;
            }
            return false;
        }
        private void ForumİstatistikleriniGüncelle(int forumId)
        {
            if (forumId == 0)
            {
                return;
            }
            var forum = ForumAlId(forumId);
            if (forum == null)
            {
                return;
            }
            var sorguSayfaSayısı = from ft in _forumSayfaDepo.Tablo
                                 where ft.ForumId == forumId
                                 select ft.Id;
            int toplamSayfalar = sorguSayfaSayısı.Count();
            var sorguGirdiSayısı = from ft in _forumSayfaDepo.Tablo
                                join fp in _forumGirdisiDepo.Tablo on ft.Id equals fp.SayfaId
                                where ft.ForumId == forumId
                                select fp.Id;
            int toplamGirdiler = sorguGirdiSayısı.Count();
            int sonSayfaId = 0;
            int sonGirdiId = 0;
            int sonGirdiKullanıcıId = 0;
            DateTime? sonGirdiTarihi = null;
            var sorguSonDeğerler = from ft in _forumSayfaDepo.Tablo
                                  join fp in _forumGirdisiDepo.Tablo on ft.Id equals fp.SayfaId
                                  where ft.ForumId == forumId
                                  orderby fp.OluşturulmaTarihi descending, ft.OluşturulmaTarihi descending
                                  select new
                                  {
                                      SonSayfaId = ft.Id,
                                      SonGirdiId = fp.Id,
                                      SonGirdiKullanıcıId = fp.KullanıcıId,
                                      SonGirdiTarihi = fp.OluşturulmaTarihi
                                  };
            var sonDeğerler = sorguSonDeğerler.FirstOrDefault();
            if (sonDeğerler != null)
            {
                sonSayfaId = sonDeğerler.SonSayfaId;
                sonGirdiId = sonDeğerler.SonGirdiId;
                sonGirdiKullanıcıId = sonDeğerler.SonGirdiKullanıcıId;
                sonGirdiTarihi = sonDeğerler.SonGirdiTarihi;
            }

            forum.SayfaSayısı = toplamSayfalar;
            forum.PostSayısı = toplamGirdiler;
            forum.SonSayfaId = sonSayfaId;
            forum.SonPostId = sonGirdiId;
            forum.SonPustKullanıcıId = sonGirdiKullanıcıId;
            forum.SonPostZamanı = sonGirdiTarihi;
            ForumGüncelle(forum);
        }
        private void ForumSayfasıİstatistikleriniGüncelle(int forumSayfaId)
        {
            if (forumSayfaId == 0)
            {
                return;
            }
            var forumSayfası = SayfaAlId(forumSayfaId);
            if (forumSayfası == null)
            {
                return;
            }
            var sorguGirdiSayısı = from fp in _forumGirdisiDepo.Tablo
                                where fp.SayfaId == forumSayfaId
                                select fp.Id;
            int girdiSayısı = sorguGirdiSayısı.Count();
            int sonGirdiId = 0;
            int sonGirdiKullanıcıId = 0;
            DateTime? sonGirdiTarihi = null;
            var sorguSonDeğerler = from fp in _forumGirdisiDepo.Tablo
                                  where fp.SayfaId == forumSayfaId
                                   orderby fp.OluşturulmaTarihi descending
                                  select new
                                  {
                                      SonGirdiId = fp.Id,
                                      SonGirdiKullanıcıId = fp.KullanıcıId,
                                      SonGirdiTarihi = fp.OluşturulmaTarihi
                                  };
            var sonDeğerler = sorguSonDeğerler.FirstOrDefault();
            if (sonDeğerler != null)
            {
                sonGirdiId = sonDeğerler.SonGirdiId;
                sonGirdiKullanıcıId = sonDeğerler.SonGirdiKullanıcıId;
                sonGirdiTarihi = sonDeğerler.SonGirdiTarihi;
            }
            forumSayfası.PostSayısı = girdiSayısı;
            forumSayfası.SonPostId = sonGirdiId;
            forumSayfası.SonPostKullanıcıId = sonGirdiKullanıcıId;
            forumSayfası.SonPostZamanı = sonGirdiTarihi;
            SayfaGüncelle(forumSayfası);
        }
        private void KullanıcıİstatistikleriniGüncelle(int kullanıcıId)
        {
            if (kullanıcıId == 0)
            {
                return;
            }
            var kullanıcı = _kullanıcıServisi.KullanıcıAlId(kullanıcıId);
            if (kullanıcı == null)
            {
                return;
            }
            var sorgu = from fp in _forumGirdisiDepo.Tablo
                        where fp.KullanıcıId == kullanıcıId
                        select fp.Id;
            int girdiSayısı = sorgu.Count();
            _genelÖznitelikServisi.ÖznitelikKaydet(kullanıcı, SistemKullanıcıÖznitelikAdları.ForumGirdiSayısı, girdiSayısı);
        }
        /*
        private bool Kayıtlı(Kullanıcı kullanıcı)
        {
            if (kullanıcı.Kayıtlı())
            {
                return true;
            }
            return false;
        }*/
        public virtual ForumAboneliği AbonelikAlId(int forumAboneliğiId)
        {
            if (forumAboneliğiId == 0)
                return null;
            return _forumAboneliğiDepo.AlId(forumAboneliğiId);
        }

        public virtual void AbonelikEkle(ForumAboneliği forumAboneliği)
        {
            if (forumAboneliği == null)
            {
                throw new ArgumentNullException("forumAboneliği");
            }
            _forumAboneliğiDepo.Ekle(forumAboneliği);
            _olayYayınlayıcı.OlayEklendi(forumAboneliği);
        }

        public virtual void AbonelikGüncelle(ForumAboneliği forumAboneliği)
        {
            if (forumAboneliği == null)
            {
                throw new ArgumentNullException("forumAboneliği");
            }
            _forumAboneliğiDepo.Güncelle(forumAboneliği);
            _olayYayınlayıcı.OlayGüncellendi(forumAboneliği);
        }

        public virtual void AbonelikSil(ForumAboneliği forumAboneliği)
        {
            if (forumAboneliği == null)
            {
                throw new ArgumentNullException("forumSubscription");
            }
            _forumAboneliğiDepo.Sil(forumAboneliği);
            _olayYayınlayıcı.OlaySilindi(forumAboneliği);
        }

        public virtual ISayfalıListe<ForumSayfası> AktifSayfalarıAl(int forumId = 0, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu1 = from ft in _forumSayfaDepo.Tablo
                         where
                         (forumId == 0 || ft.ForumId == forumId) &&
                         (ft.SonPostZamanı.HasValue)
                         select ft.Id;

            var sorgu2 = from ft in _forumSayfaDepo.Tablo
                         where sorgu1.Contains(ft.Id)
                         orderby ft.SonPostZamanı descending
                         select ft;

            var sayfalar = new SayfalıListe<ForumSayfası>(sorgu2, sayfaIndeksi, sayfaBüyüklüğü);
            return sayfalar;
        }

        public virtual Forum ForumAlId(int forumId)
        {
            if (forumId == 0)
                return null;
            return _forumDepo.AlId(forumId);
        }

        public virtual void ForumEkle(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }
            _forumDepo.Ekle(forum);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(forum);
        }

        public virtual ForumGirdisiOyu ForumGirdisiOyuAl(int girdiId, Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                return null;
            return _forumGirdiOyuDepo.Tablo.FirstOrDefault(pv => pv.ForumGirdisiId == girdiId && pv.KullanıcıId == kullanıcı.Id);
        }

        public virtual void ForumGirdisiOyuEkle(ForumGirdisiOyu girdiOyu)
        {
            if (girdiOyu == null)
                throw new ArgumentNullException("girdiOyu");
            _forumGirdiOyuDepo.Ekle(girdiOyu);
            var post = this.GirdiAlId(girdiOyu.ForumGirdisiId);
            post.OySayısı = girdiOyu.Yukarı ? ++post.OySayısı : --post.OySayısı;
            this.GirdiGüncelle(post);
            _olayYayınlayıcı.OlayEklendi(girdiOyu);
        }

        public virtual void ForumGirdisiOyuGüncelle(ForumGirdisiOyu girdiOyu)
        {
            if (girdiOyu == null)
                throw new ArgumentNullException("girdiOyu");
            _forumGirdiOyuDepo.Güncelle(girdiOyu);
            _olayYayınlayıcı.OlayGüncellendi(girdiOyu);
        }

        public virtual int ForumGirdisiOyuSayısıAl(Kullanıcı kullanıcı, DateTime oluşturulmaTarihi)
        {
            if (kullanıcı == null)
                return 0;
            return _forumGirdiOyuDepo.Tablo.Count(pv => pv.KullanıcıId == kullanıcı.Id && pv.OluşturulmaTarihi > oluşturulmaTarihi);
        }

        public virtual void ForumGirdisiOyuSil(ForumGirdisiOyu girdiOyu)
        {
            if (girdiOyu == null)
                throw new ArgumentNullException("girdiOyu");
            _forumGirdiOyuDepo.Güncelle(girdiOyu);
            var post = this.GirdiAlId(girdiOyu.ForumGirdisiId);
            post.OySayısı = girdiOyu.Yukarı ? --post.OySayısı : ++post.OySayısı;
            this.GirdiGüncelle(post);
            _olayYayınlayıcı.OlayGüncellendi(girdiOyu);
        }

        public virtual ForumGrubu ForumGrubuAlId(int forumGrupId)
        {
            if (forumGrupId == 0)
            {
                return null;
            }
            return _forumGrupDepo.AlId(forumGrupId);
        }

        public virtual void ForumGrubuEkle(ForumGrubu forumGrubu)
        {
            if (forumGrubu == null)
            {
                throw new ArgumentNullException("forumGrubu");
            }
            _forumGrupDepo.Ekle(forumGrubu);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(forumGrubu);
        }

        public virtual void ForumGrubuGüncelle(ForumGrubu forumGrubu)
        {
            if (forumGrubu == null)
            {
                throw new ArgumentNullException("forumGrubu");
            }
            _forumGrupDepo.Güncelle(forumGrubu);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(forumGrubu);
        }

        public virtual void ForumGrubunuSil(ForumGrubu forumGrubu)
        {
            if (forumGrubu == null)
            {
                throw new ArgumentNullException("forumGrubu");
            }

            _forumGrupDepo.Sil(forumGrubu);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(forumGrubu);
        }

        public virtual void ForumGüncelle(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }
            _forumDepo.Güncelle(forum);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(forum);
        }

        public virtual void ForumSil(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }
            //sayfaları sil
            var sorguSayfaIdleri = from ft in _forumSayfaDepo.Tablo
                                where ft.ForumId == forum.Id
                                select ft.Id;
            var sorguFs1 = from fs in _forumAboneliğiDepo.Tablo
                           where sorguSayfaIdleri.Contains(fs.SayfaId)
                           select fs;
            foreach (var fs in sorguFs1.ToList())
            {
                _forumAboneliğiDepo.Sil(fs);
                _olayYayınlayıcı.OlaySilindi(fs);
            }

            //abonelik sil
            var queryFs2 = from fs in _forumAboneliğiDepo.Tablo
                           where fs.ForumId == forum.Id
                           select fs;
            foreach (var fs2 in queryFs2.ToList())
            {
                _forumAboneliğiDepo.Sil(fs2);
                _olayYayınlayıcı.OlaySilindi(fs2);
            }

            //delete forum
            _forumDepo.Sil(forum);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(forum);
        }

        public virtual ForumGirdisi GirdiAlId(int forumGirdisiId)
        {
            if (forumGirdisiId == 0)
                return null;
            return _forumGirdisiDepo.AlId(forumGirdisiId);
        }

        public virtual void GirdiEkle(ForumGirdisi forumGirdisi, bool bildirimGönder)
        {
            if (forumGirdisi == null)
            {
                throw new ArgumentNullException("forumGirdisi");
            }
            _forumGirdisiDepo.Ekle(forumGirdisi);
            int kullanıcıId = forumGirdisi.KullanıcıId;
            var forumSayfası = this.SayfaAlId(forumGirdisi.SayfaId);
            int forumId = forumSayfası.ForumId;
            ForumSayfasıİstatistikleriniGüncelle(forumGirdisi.SayfaId);
            ForumİstatistikleriniGüncelle(forumId);
            KullanıcıİstatistikleriniGüncelle(kullanıcıId);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(forumGirdisi);
            if (bildirimGönder)
            {
                var forum = forumSayfası.Forum;
                var abonelikler = TümAbonelikleriAl(sayfaId: forumSayfası.Id);
                int friendlySayfaIndeksi = SayfaIndeksiHesapla(forumGirdisi.SayfaId,
                    _forumAyarları.GirdiSayfaBüyüklüğü > 0 ? _forumAyarları.GirdiSayfaBüyüklüğü : 10,
                    forumGirdisi.Id) + 1;

                foreach (ForumAboneliği abonelik in abonelikler)
                {
                    if (abonelik.KullanıcıId == forumGirdisi.KullanıcıId)
                    {
                        continue;
                    }

                    if (!String.IsNullOrEmpty(abonelik.Kullanıcı.Email))
                    {
                        _mesajServisi.YeniForumGirdisiMesajıGönder(abonelik.Kullanıcı, forumGirdisi,
                            forumSayfası, forum, friendlySayfaIndeksi);
                    }
                }
            }
        }

        public virtual void GirdiGüncelle(ForumGirdisi forumGirdisi)
        {
            if (forumGirdisi == null)
            {
                throw new ArgumentNullException("forumPost");
            }
            _forumGirdisiDepo.Güncelle(forumGirdisi);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(forumGirdisi);
        }

        public virtual void GirdiSil(ForumGirdisi forumGirdisi)
        {
            if (forumGirdisi == null)
            {
                throw new ArgumentNullException("forumGirdisi");
            }
            int forumSayfaId = forumGirdisi.SayfaId;
            int kullanıcıId = forumGirdisi.KullanıcıId;
            var forumSayfası = this.SayfaAlId(forumSayfaId);
            int forumId = forumSayfası.ForumId;
            bool sayfaSil = false;
            ForumGirdisi ilkGirdi = forumSayfası.İlkGirdiAl(this);
            if (ilkGirdi != null && ilkGirdi.Id == forumGirdisi.Id)
            {
                sayfaSil = true;
            }
            _forumGirdisiDepo.Sil(forumGirdisi);
            if (sayfaSil)
            {
                SayfaSil(forumSayfası);
            }
            if (!sayfaSil)
            {
                ForumSayfasıİstatistikleriniGüncelle(forumSayfaId);
            }
            ForumİstatistikleriniGüncelle(forumId);
            KullanıcıİstatistikleriniGüncelle(kullanıcıId);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(forumGirdisi);
        }

        public virtual bool KullanıcıAboneOlabilir(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest())
            {
                return false;
            }
            return true;
        }

        public virtual bool KullanıcıSayfaDüzenleyebilir(Kullanıcı kullanıcı, ForumSayfası sayfa)
        {
            if (sayfa == null)
            {
                return false;
            }
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest())
            {
                return false;
            }
            if (Kayıtlı(kullanıcı))
            {
                return true;
            }
            if (_forumAyarları.KullanıcılarınGirdiDüzenlemesiEtkin)
            {
                bool sayfaSahibi = kullanıcı.Id == sayfa.KullanıcıId;
                return sayfaSahibi;
            }
            return false;
        }

        public virtual bool KullanıcıSayfaGirdisiDüzenleyebilir(Kullanıcı kullanıcı, ForumGirdisi post)
        {
            if (post == null)
            {
                return false;
            }
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest())
            {
                return false;
            }
            if (Kayıtlı(kullanıcı))
            {
                return true;
            }
            if (_forumAyarları.KullanıcılarınGirdiDüzenlemesiEtkin)
            {
                bool ownPost = kullanıcı.Id == post.KullanıcıId;
                return ownPost;
            }
            return false;
        }

        public virtual bool KullanıcıSayfaGirdisiOluşturabilir(Kullanıcı kullanıcı, ForumSayfası sayfa)
        {
            if (sayfa == null)
            {
                return false;
            }
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest() && !_forumAyarları.ZiyaretçilerinGirdiEklemesiEtkin)
            {
                return false;
            }
            return true;
        }

        public virtual bool KullanıcıSayfaGirdisiSilebilir(Kullanıcı kullanıcı, ForumGirdisi post)
        {
            if (post == null)
            {
                return false;
            }
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest())
            {
                return false;
            }
            if (Kayıtlı(kullanıcı))
            {
                return true;
            }
            if (_forumAyarları.KullanıcılarınGirdiSilmesiEtkin)
            {
                bool ownPost = kullanıcı.Id == post.KullanıcıId;
                return ownPost;
            }
            return false;
        }

        public virtual bool KullanıcıSayfaOluşturabilir(Kullanıcı kullanıcı, Forum forum)
        {
            if (forum == null)
            {
                return false;
            }
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest() && !_forumAyarları.ZiyaretçilerinSayfaEklemesiEtkin)
            {
                return false;
            }
            if (Kayıtlı(kullanıcı))
            {
                return true;
            }
            return true;
        }

        public virtual bool KullanıcıSayfaSilebilir(Kullanıcı kullanıcı, ForumSayfası sayfa)
        {
            if (sayfa == null)
            {
                return false;
            }
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest())
            {
                return false;
            }
            if (Kayıtlı(kullanıcı))
            {
                return true;
            }
            if (_forumAyarları.KullanıcılarınGirdiSilmesiEtkin)
            {
                bool ownTopic = kullanıcı.Id == sayfa.KullanıcıId;
                return ownTopic;
            }
            return false;
        }

        public virtual bool KullanıcıSayfaTaşıyabilir(Kullanıcı kullanıcı, ForumSayfası sayfa)
        {
            if (sayfa == null)
            {
                return false;
            }
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest())
            {
                return false;
            }
            if (Kayıtlı(kullanıcı))
            {
                return true;
            }
            return false;
        }

        public virtual bool KullanıcıSayfaÖnceliğiDüzenleyebilir(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
            {
                return false;
            }
            if (kullanıcı.IsGuest())
            {
                return false;
            }
            if (Kayıtlı(kullanıcı))
            {
                return true;
            }
            return false;
        }

        public virtual ForumSayfası SayfaAlId(int forumSayfasıId)
        {
            return SayfaAlId(forumSayfasıId, false);
        }

        public virtual ForumSayfası SayfaAlId(int forumSayfasıId, bool görüntülemeleriArtır)
        {
            if (forumSayfasıId == 0)
                return null;

            var forumSayfası = _forumSayfaDepo.AlId(forumSayfasıId);
            if (forumSayfası == null)
                return null;

            if (görüntülemeleriArtır)
            {
                forumSayfası.Görüntülenme = ++forumSayfası.Görüntülenme;
                SayfaGüncelle(forumSayfası);
            }
            return forumSayfası;
        }

        public virtual void SayfaEkle(ForumSayfası forumSayfası, bool bildirimGönder)
        {
            if (forumSayfası == null)
            {
                throw new ArgumentNullException("forumSayfası");
            }
            _forumSayfaDepo.Ekle(forumSayfası);
            ForumİstatistikleriniGüncelle(forumSayfası.ForumId);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(forumSayfası);
            if (bildirimGönder)
            {
                var forum = forumSayfası.Forum;
                var abonelikler = TümAbonelikleriAl(forumId: forum.Id);

                foreach (var abonelik in abonelikler)
                {
                    if (abonelik.KullanıcıId == forumSayfası.KullanıcıId)
                    {
                        continue;
                    }

                    if (!String.IsNullOrEmpty(abonelik.Kullanıcı.Email))
                    {
                        _mesajServisi.YeniForumSayfasıMesajıGönder(abonelik.Kullanıcı, forumSayfası,
                            forum);
                    }
                }
            }
        }

        public virtual void SayfaGüncelle(ForumSayfası forumSayfası)
        {
            if (forumSayfası == null)
            {
                throw new ArgumentNullException("forumSayfası");
            }
            _forumSayfaDepo.Güncelle(forumSayfası);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(forumSayfası);
        }

        public virtual int SayfaIndeksiHesapla(int forumSayfasıId, int sayfaBüyüklüğü, int postId)
        {
            int sayfaIndeksi = 0;
            var forumPosts = TümGirdileriAl(forumSayfasıId: forumSayfasıId, azalanSırala: true);

            for (int i = 0; i < forumPosts.TotalCount; i++)
            {
                if (forumPosts[i].Id == postId)
                {
                    if (sayfaBüyüklüğü > 0)
                    {
                        sayfaIndeksi = i / sayfaBüyüklüğü;
                    }
                }
            }
            return sayfaIndeksi;
        }

        public virtual void SayfaSil(ForumSayfası forumSayfası)
        {
            if (forumSayfası == null)
            {
                throw new ArgumentNullException("forumSayfası");
            }

            int kullanıcıId = forumSayfası.KullanıcıId;
            int forumId = forumSayfası.ForumId;

            _forumSayfaDepo.Sil(forumSayfası);
            var sorguFs = from ft in _forumAboneliğiDepo.Tablo
                          where ft.SayfaId == forumSayfası.Id
                          select ft;
            var forumAbonelikleri = sorguFs.ToList();
            foreach (var fs in forumAbonelikleri)
            {
                _forumAboneliğiDepo.Sil(fs);
                _olayYayınlayıcı.OlaySilindi(fs);
            }
            ForumİstatistikleriniGüncelle(forumId);
            KullanıcıİstatistikleriniGüncelle(kullanıcıId);
            _önbellekYönetici.KalıpİleSil(FORUMGRUP_PATTERN_KEY);
            _önbellekYönetici.KalıpİleSil(FORUM_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(forumSayfası);
        }

        public virtual ForumSayfası SayfayıTaşı(int forumSayfasıId, int yeniForumId)
        {
            var forumSayfası = SayfaAlId(forumSayfasıId);
            if (forumSayfası == null)
                return null;

            if (this.KullanıcıSayfaTaşıyabilir(_workContext.MevcutKullanıcı, forumSayfası))
            {
                int öncekiForumId = forumSayfası.ForumId;
                var yeniForum = ForumAlId(yeniForumId);

                if (yeniForum != null)
                {
                    if (öncekiForumId != yeniForumId)
                    {
                        forumSayfası.ForumId = yeniForum.Id;
                        forumSayfası.GüncellenmeTarihi = DateTime.UtcNow;
                        SayfaGüncelle(forumSayfası);

                        //update forum stats
                        ForumİstatistikleriniGüncelle(öncekiForumId);
                        ForumİstatistikleriniGüncelle(yeniForumId);
                    }
                }
            }
            return forumSayfası;
        }

        public virtual ISayfalıListe<ForumAboneliği> TümAbonelikleriAl(int kullanıcıId = 0, int forumId = 0, int sayfaId = 0, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var fsSorgu = from fs in _forumAboneliğiDepo.Tablo
                          join c in _kullanıcıDepo.Tablo on fs.KullanıcıId equals c.Id
                          where
                          (kullanıcıId == 0 || fs.KullanıcıId == kullanıcıId) &&
                          (forumId == 0 || fs.ForumId == forumId) &&
                          (sayfaId == 0 || fs.SayfaId == sayfaId) &&
                          (c.Aktif && !c.Silindi)
                          select fs.AbonelikGuid;

            var sorgu = from fs in _forumAboneliğiDepo.Tablo
                        where fsSorgu.Contains(fs.AbonelikGuid)
                        orderby fs.OluşturulmaTarihi descending, fs.AbonelikGuid descending
                        select fs;

            var forumAbonelikleri = new SayfalıListe<ForumAboneliği>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return forumAbonelikleri;
        }

        public virtual IList<ForumGrubu> TümForumGruplarınıAl()
        {
            string key = string.Format(FORUMGRUP_ALL_KEY);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = from fg in _forumGrupDepo.Tablo
                            orderby fg.GörüntülenmeSırası, fg.Id
                            select fg;
                return sorgu.ToList();
            });
        }

        public virtual IList<Forum> TümForumlarıAlGrupId(int forumGrupId)
        {
            string key = string.Format(FORUM_ALLBYFORUMGROUPID_KEY, forumGrupId);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = from f in _forumDepo.Tablo
                            orderby f.GörüntülenmeSırası, f.Id
                            where f.ForumGrubuId == forumGrupId
                            select f;
                var forumlar = sorgu.ToList();
                return forumlar;
            });
        }

        public virtual ISayfalıListe<ForumGirdisi> TümGirdileriAl(int forumSayfasıId = 0, int kullanıcıId = 0, string anahtarKelimeler = "", bool azalanSırala = false, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _forumGirdisiDepo.Tablo;
            if (forumSayfasıId > 0)
            {
                sorgu = sorgu.Where(fp => forumSayfasıId == fp.SayfaId);
            }
            if (kullanıcıId > 0)
            {
                sorgu = sorgu.Where(fp => kullanıcıId == fp.KullanıcıId);
            }
            if (!String.IsNullOrEmpty(anahtarKelimeler))
            {
                sorgu = sorgu.Where(fp => fp.Yazı.Contains(anahtarKelimeler));
            }

            sorgu = azalanSırala ?
                sorgu.OrderBy(fp => fp.OluşturulmaTarihi).ThenBy(fp => fp.Id) :
                sorgu.OrderByDescending(fp => fp.OluşturulmaTarihi).ThenBy(fp => fp.Id);

            var forumGirdileri = new SayfalıListe<ForumGirdisi>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return forumGirdileri;
        }

        public virtual ISayfalıListe<ForumGirdisi> TümGirdileriAl(int forumSayfasıId = 0, int kullanıcıId = 0, string anahtarKelimeler = "", int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            return TümGirdileriAl(forumSayfasıId, kullanıcıId, anahtarKelimeler, true,
                sayfaIndeksi, sayfaBüyüklüğü);
        }

        public virtual ISayfalıListe<ForumSayfası> TümSayfalarıAl(int forumId = 0, int kullanıcıId = 0, string anahtarKelimeler = "", ForumAramaTipi aramaTipi = ForumAramaTipi.Tümü, int günSınırı = 0, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            DateTime? tarihSınırı = null;
            if (günSınırı > 0)
            {
                tarihSınırı = DateTime.UtcNow.AddDays(-günSınırı);
            }
            bool anahtarKelimeleriAra = !String.IsNullOrEmpty(anahtarKelimeler);
            bool sayfaBaşlıklarınıAra = aramaTipi == ForumAramaTipi.Tümü || aramaTipi == ForumAramaTipi.SadeceSayfaBaşlığı;
            bool girdiAra = aramaTipi == ForumAramaTipi.Tümü || aramaTipi == ForumAramaTipi.GirdiMesajıSadece;
            var sorgu1 = from ft in _forumSayfaDepo.Tablo
                         join fp in _forumGirdisiDepo.Tablo on ft.Id equals fp.SayfaId
                         where
                         (forumId == 0 || ft.ForumId == forumId) &&
                         (kullanıcıId == 0 || ft.KullanıcıId == kullanıcıId) &&
                         (
                            !anahtarKelimeleriAra ||
                            (sayfaBaşlıklarınıAra && ft.Konu.Contains(anahtarKelimeler)) ||
                            (girdiAra && fp.Yazı.Contains(anahtarKelimeler))) &&
                         (!tarihSınırı.HasValue || tarihSınırı.Value <= ft.SonPostZamanı)
                         select ft.Id;

            var sorgu2 = from ft in _forumSayfaDepo.Tablo
                         where sorgu1.Contains(ft.Id)
                         orderby ft.SayfaTipiId descending, ft.SonPostZamanı descending, ft.Id descending
                         select ft;

            var sayfalar = new SayfalıListe<ForumSayfası>(sorgu2, sayfaIndeksi, sayfaBüyüklüğü);
            return sayfalar;
        }

        public virtual ISayfalıListe<ÖzelMesaj> TümÖzelMesajlarıAl(int siteId, int kulanıcıdanId, int kullanıcıyaId, bool? okundu, bool? yazarTarafındanSilindi, bool? alıcıTarafındanSilindi, string anahtarKelimeler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _forumÖzelMesajDepo.Tablo;
            if (siteId > 0)
                sorgu = sorgu.Where(pm => siteId == pm.SiteId);
            if (kulanıcıdanId > 0)
                sorgu = sorgu.Where(pm => kulanıcıdanId == pm.KullanıcıdanId);
            if (kullanıcıyaId > 0)
                sorgu = sorgu.Where(pm => kullanıcıyaId == pm.KullanıcıyaId);
            if (okundu.HasValue)
                sorgu = sorgu.Where(pm => okundu.Value == pm.Okundu);
            if (yazarTarafındanSilindi.HasValue)
                sorgu = sorgu.Where(pm => yazarTarafındanSilindi.Value == pm.YazarTarafındanSilindi);
            if (alıcıTarafındanSilindi.HasValue)
                sorgu = sorgu.Where(pm => alıcıTarafındanSilindi.Value == pm.AlıcıTarafındanSilindi);
            if (!String.IsNullOrEmpty(anahtarKelimeler))
            {
                sorgu = sorgu.Where(pm => pm.Konu.Contains(anahtarKelimeler));
                sorgu = sorgu.Where(pm => pm.Mesaj.Contains(anahtarKelimeler));
            }
            sorgu = sorgu.OrderByDescending(pm => pm.OluşturulmaTarihi);

            var privateMessages = new SayfalıListe<ÖzelMesaj>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return privateMessages;
        }

        public virtual ÖzelMesaj ÖzelMesajAlId(int özelMesajId)
        {
            if (özelMesajId == 0)
                return null;
            return _forumÖzelMesajDepo.AlId(özelMesajId);
        }

        public virtual void ÖzelMesajEkle(ÖzelMesaj özelMesaj)
        {
            if (özelMesaj == null)
            {
                throw new ArgumentNullException("özelMesaj");
            }
            _forumÖzelMesajDepo.Ekle(özelMesaj);
            _olayYayınlayıcı.OlayEklendi(özelMesaj);
            var kullanıcıya = _kullanıcıServisi.KullanıcıAlId(özelMesaj.KullanıcıyaId);
            if (kullanıcıya == null)
            {
                throw new Hata("Alıcı mevcut değil");
            }
            _genelÖznitelikServisi.ÖznitelikKaydet(kullanıcıya, SistemKullanıcıÖznitelikAdları.YeniÖzelMesajBilgisi, false, özelMesaj.SiteId);
            if (_forumAyarları.ÖzelMesajlarıBildir)
            {
                _mesajServisi.ÖzelMesajBildirimiGönder(özelMesaj);
            }
        }

        public virtual void ÖzelMesajGüncelle(ÖzelMesaj özelMesaj)
        {
            if (özelMesaj == null)
                throw new ArgumentNullException("özelMesaj");
            if (özelMesaj.YazarTarafındanSilindi && özelMesaj.AlıcıTarafındanSilindi)
            {
                _forumÖzelMesajDepo.Sil(özelMesaj);
                _olayYayınlayıcı.OlaySilindi(özelMesaj);
            }
            else
            {
                _forumÖzelMesajDepo.Güncelle(özelMesaj);
                _olayYayınlayıcı.OlayGüncellendi(özelMesaj);
            }
        }

        public virtual void ÖzelMesajSil(ÖzelMesaj özelMesaj)
        {
            if (özelMesaj == null)
            {
                throw new ArgumentNullException("özelMesaj");
            }
            _forumÖzelMesajDepo.Sil(özelMesaj);
            _olayYayınlayıcı.OlaySilindi(özelMesaj);
        }
    }
}
