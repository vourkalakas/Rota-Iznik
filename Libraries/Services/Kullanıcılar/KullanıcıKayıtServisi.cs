using System;
using System.Linq;
using Core;
using Core.Domain.Kullanıcılar;
using Services.Genel;
using Services.Olaylar;
using Services.Güvenlik;
using Services.Siteler;

namespace Services.Kullanıcılar
{
    public partial class KullanıcıKayıtServisi : IKullanıcıKayıtServisi
    {
        #region Fields

        private readonly IKullanıcıServisi _kullanıcıServisi;
        private readonly IŞifrelemeServisi _şifrelemeServisi;
        private readonly IAbonelikServisi _abonelikServisi;
        private readonly ISiteServisi _siteServisi;
        //private readonly IÖdülPuanıServisi _ödülPuanıServisi;
        private readonly IGenelÖznitelikServisi _genelÖznitelikServisi;
        private readonly IWorkContext _workContext;
        //private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        //private readonly ÖdülPuanıAarları _ödülPuanıAyarları;
        private readonly KullanıcıAyarları _kullanıcıAyarları;

        #endregion

        #region Ctor
        public KullanıcıKayıtServisi(IKullanıcıServisi kullanıcıServisi,
            IŞifrelemeServisi şifrelemeServisi,
            IAbonelikServisi abonelikServisi,
            ISiteServisi siteServisi,
            //IÖdülPuanıServisi ödülPuanıServisi,
            IWorkContext workContext,
            IGenelÖznitelikServisi genelÖznitelikServisi,
            //IWorkflowMessageService workflowMessageService,
            IOlayYayınlayıcı olayYayınlayıcı,
            //ÖdülPuanıAarları ödülPuanıAyarları,
            KullanıcıAyarları kullanıcıAyarları)
        {
            this._kullanıcıServisi = kullanıcıServisi;
            this._şifrelemeServisi = şifrelemeServisi;
            this._abonelikServisi = abonelikServisi;
            this._siteServisi = siteServisi;
            this._genelÖznitelikServisi = genelÖznitelikServisi;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._kullanıcıAyarları = kullanıcıAyarları;
        }

        #endregion

        #region Utilities
        protected bool ŞifreUyuşturması(KullanıcıŞifre kullanıcıŞifre, string girilenŞifre)
        {
            if (kullanıcıŞifre == null || string.IsNullOrEmpty(girilenŞifre))
                return false;

            var kaydedilenŞifre = string.Empty;
            switch (kullanıcıŞifre.ŞifreFormatı)
            {
                case ŞifreFormatı.Temiz:
                    kaydedilenŞifre = girilenŞifre;
                    break;
                case ŞifreFormatı.Encrypted:
                    kaydedilenŞifre = _şifrelemeServisi.TextŞifrele(girilenŞifre);
                    break;
                case ŞifreFormatı.Hashed:
                    kaydedilenŞifre = _şifrelemeServisi.ŞifreHashOluştur(girilenŞifre, kullanıcıŞifre.ŞifreSalt, _kullanıcıAyarları.HashŞifreFormatı);
                    break;
            }

            return kullanıcıŞifre.Şifre.Equals(kaydedilenŞifre);
        }

        #endregion

        #region Methods
        public virtual KullanıcıGirişSonuçları KullanıcıDoğrula(string kullanıcıAdıVeyaEmail, string şifre)
        {
            var kullanıcı = _kullanıcıAyarları.KullanıcıAdlarıEtkin ?
                _kullanıcıServisi.KullanıcıAlKullanıcıAdı(kullanıcıAdıVeyaEmail) :
                _kullanıcıServisi.KullanıcıAlEmail(kullanıcıAdıVeyaEmail);

            if (kullanıcı == null)
                return KullanıcıGirişSonuçları.KullanıcıMevcutDeğil;
            if (kullanıcı.Silindi)
                return KullanıcıGirişSonuçları.Silindi;
            if (!kullanıcı.Aktif)
                return KullanıcıGirişSonuçları.AktifDeğil;
            //sadece kayıtlı olanlar girebilir
            if (!kullanıcı.IsRegistered())
                return KullanıcıGirişSonuçları.KayıtlıDeğil;
            //check whether a kullanıcı is locked out
            if (kullanıcı.ŞuTarihdenBeriGirişYapamıyor.HasValue && kullanıcı.ŞuTarihdenBeriGirişYapamıyor.Value > DateTime.UtcNow)
                return KullanıcıGirişSonuçları.Kilitlendi;

            if (!ŞifreUyuşturması(_kullanıcıServisi.MevcutŞifreAl(kullanıcı.Id), şifre))
            {
                //yanlış şifre
                kullanıcı.HatalıGirişSayısı++;
                if (_kullanıcıAyarları.HatalıŞifreDenemesi > 0 &&
                    kullanıcı.HatalıGirişSayısı >= _kullanıcıAyarları.HatalıŞifreDenemesi)
                {
                    //kilitle
                    kullanıcı.ŞuTarihdenBeriGirişYapamıyor = DateTime.UtcNow.AddMinutes(_kullanıcıAyarları.HatalıŞifredeKilitDakikası);
                    //sayacı sıfırla
                    kullanıcı.HatalıGirişSayısı = 0;
                }
                _kullanıcıServisi.KullanıcıGüncelle(kullanıcı);

                return KullanıcıGirişSonuçları.HatalıŞifre;
            }

            //giriş detaylarını güncelle
            kullanıcı.HatalıGirişSayısı = 0;
            kullanıcı.ŞuTarihdenBeriGirişYapamıyor = null;
            kullanıcı.GirişGerekli = false;
            kullanıcı.SonGirişTarihi = DateTime.UtcNow;
            _kullanıcıServisi.KullanıcıGüncelle(kullanıcı);

            return KullanıcıGirişSonuçları.Başarılı;
        }
        public virtual KullanıcıKayıtSonuçları KullanıcıKaydet(KullanıcıKayıtİsteği istek)
        {
            if (istek == null)
                throw new ArgumentNullException("istek");

            if (istek.Kullanıcı == null)
                throw new ArgumentException("Mevcut kullanıcı yüklenemedi");

            var sonuç = new KullanıcıKayıtSonuçları();
            if (istek.Kullanıcı.AramaMotoruHesabı())
            {
                sonuç.HataEkle("Arama motoru kaydedilemedi");
                return sonuç;
            }
            if (istek.Kullanıcı.ArkaPlanGöreviHesabı())
            {
                sonuç.HataEkle("Arka plan görevi hesabı kaydedilemedi");
                return sonuç;
            }
            if (istek.Kullanıcı.IsRegistered())
            {
                sonuç.HataEkle("Mevcut kullanıcı zaten kayıtlı");
                return sonuç;
            }
            if (String.IsNullOrEmpty(istek.Email))
            {
                sonuç.HataEkle("E-mail sağlanamadı");
                return sonuç;
            }
            if (!GenelYardımcı.GeçerliMail(istek.Email))
            {
                sonuç.HataEkle("Hatalı E-mail");
                return sonuç;
            }
            if (String.IsNullOrWhiteSpace(istek.Şifre))
            {
                sonuç.HataEkle("Şifre sağlanamadı");
                return sonuç;
            }
            if (_kullanıcıAyarları.KullanıcıAdlarıEtkin)
            {
                if (String.IsNullOrEmpty(istek.KullanıcıAdı))
                {
                    sonuç.HataEkle("Kullanıcı adı sağlanamadı");
                    return sonuç;
                }
            }

            //benzersiz kullanıcıyı doğrulama
            if (_kullanıcıServisi.KullanıcıAlEmail(istek.Email) != null)
            {
                sonuç.HataEkle("E-Mail adresi zaten mevcut");
                return sonuç;
            }
            if (_kullanıcıAyarları.KullanıcıAdlarıEtkin)
            {
                if (_kullanıcıServisi.KullanıcıAlKullanıcıAdı(istek.KullanıcıAdı) != null)
                {
                    sonuç.HataEkle("Kullanıcı adı zaten mevcut");
                    return sonuç;
                }
            }

            //buradan sonra istek doğrulandı
            istek.Kullanıcı.KullanıcıAdı = istek.KullanıcıAdı;
            istek.Kullanıcı.Email = istek.Email;

            var kullanıcıŞifre = new KullanıcıŞifre
            {
                Kullanıcı = istek.Kullanıcı,
                ŞifreFormatı = istek.ŞifreFormatı,
                OluşturulmaTarihi = DateTime.UtcNow
            };
            switch (istek.ŞifreFormatı)
            {
                case ŞifreFormatı.Temiz:
                    kullanıcıŞifre.Şifre = istek.Şifre;
                    break;
                case ŞifreFormatı.Encrypted:
                    kullanıcıŞifre.Şifre = _şifrelemeServisi.TextŞifrele(istek.Şifre);
                    break;
                case ŞifreFormatı.Hashed:
                    {
                        var saltKey = _şifrelemeServisi.SaltAnahtarıOluştur(5);
                        kullanıcıŞifre.ŞifreSalt = saltKey;
                        kullanıcıŞifre.Şifre = _şifrelemeServisi.ŞifreHashOluştur(istek.Şifre, saltKey, _kullanıcıAyarları.HashŞifreFormatı);
                    }
                    break;
            }
            _kullanıcıServisi.KullanıcıŞifresiEkle(kullanıcıŞifre);

            istek.Kullanıcı.Aktif = istek.Onaylandı;

            //kayıtlı rol ekle
            var kayıtlıRol = _kullanıcıServisi.KullanıcıRolüAlSistemAdı(SistemKullanıcıRolAdları.Kayıtlı);
            if (kayıtlıRol == null)
                throw new Hata("Kayıtlı rolü eklenemedi");
            istek.Kullanıcı.KullanıcıRolleri.Add(kayıtlıRol);
            //ziyaretçi rolünü sil
            var ziyaretçiRolü = istek.Kullanıcı.KullanıcıRolleri.FirstOrDefault(cr => cr.SistemAdı == SistemKullanıcıRolAdları.Ziyaretçi);
            if (ziyaretçiRolü != null)
                istek.Kullanıcı.KullanıcıRolleri.Remove(ziyaretçiRolü);

            //Add reward points for kullanıcı registration (if enabled)
            //
            //

            _kullanıcıServisi.KullanıcıGüncelle(istek.Kullanıcı);

            //publish event
            _olayYayınlayıcı.Yayınla(new KullanıcıŞifreDeğiştirdiOlayı(kullanıcıŞifre));

            return sonuç;
        }
        public virtual ŞifreDeğiştirmeSonuçları ŞifreDeğiştir(ŞifreDeğiştirmeİsteği istek)
        {
            if (istek == null)
                throw new ArgumentNullException("istek");

            var sonuç = new ŞifreDeğiştirmeSonuçları();
            if (String.IsNullOrWhiteSpace(istek.Email))
            {
                sonuç.HataEkle("E-Mail sağlanmadı");
                return sonuç;
            }
            if (String.IsNullOrWhiteSpace(istek.YeniŞifre))
            {
                sonuç.HataEkle("Şifre sağlanmadı");
                return sonuç;
            }

            var kullanıcı = _kullanıcıServisi.KullanıcıAlEmail(istek.Email);
            if (kullanıcı == null)
            {
                sonuç.HataEkle("E-Mail bulunamadı");
                return sonuç;
            }

            if (istek.İsteğiDoğrula)
            {
                //istek geçerli değilse
                if (!ŞifreUyuşturması(_kullanıcıServisi.MevcutŞifreAl(kullanıcı.Id), istek.EskiŞifre))
                {
                    sonuç.HataEkle("Şifreler uyuşmuyor");
                    return sonuç;
                }
            }
            /*
            //kopyalama kontrol et
            if (_kullanıcıAyarları.YinelenmemişŞifreSayısı > 0)
            {
                //get some of previous passwords
                var öncekiŞifre = _kullanıcıServisi.KullanıcıŞifreleriAl(kullanıcı.Id, geriDönenŞifreler: _kullanıcıAyarları.YinelenmemişŞifreSayısı);

                var yeniŞifreÖncekiİleEşleşiyor = öncekiŞifre.Any(password => ŞifreUyuşturması(password, istek.YeniŞifre));
                if (yeniŞifreÖncekiİleEşleşiyor)
                {
                    sonuç.HataEkle("Yeni şifre bir önceki ile aynı");
                    return sonuç;
                }
            }
            */
            //buradan sonra istek doğrulandı

            var kullanıcıŞifre = new KullanıcıŞifre
            {
                Kullanıcı = kullanıcı,
                ŞifreFormatı = istek.YeniŞifreFormatı,
                OluşturulmaTarihi = DateTime.UtcNow
            };
            switch (istek.YeniŞifreFormatı)
            {
                case ŞifreFormatı.Temiz:
                    kullanıcıŞifre.Şifre = istek.YeniŞifre;
                    break;
                case ŞifreFormatı.Encrypted:
                    kullanıcıŞifre.Şifre = _şifrelemeServisi.TextŞifrele(istek.YeniŞifre);
                    break;
                case ŞifreFormatı.Hashed:
                    {
                        var saltKey = _şifrelemeServisi.SaltAnahtarıOluştur(5);
                        kullanıcıŞifre.ŞifreSalt = saltKey;
                        kullanıcıŞifre.Şifre = _şifrelemeServisi.ŞifreHashOluştur(istek.YeniŞifre, saltKey, _kullanıcıAyarları.HashŞifreFormatı);
                    }
                    break;
            }
            _kullanıcıServisi.KullanıcıŞifresiEkle(kullanıcıŞifre);

            //olay yayınla
            _olayYayınlayıcı.Yayınla(new KullanıcıŞifreDeğiştirdiOlayı(kullanıcıŞifre));

            return sonuç;
        }
        public virtual void EmailAyarla(Kullanıcı kullanıcı, string yeniEmail, bool doğrulamaGerekli)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            if (yeniEmail == null)
                throw new Hata("Email boş olamaz");

            yeniEmail = yeniEmail.Trim();
            string eskiEmail = kullanıcı.Email;

            if (!GenelYardımcı.GeçerliMail(yeniEmail))
                throw new Hata("Yeni E-Mail geçerli değil");

            if (yeniEmail.Length > 100)
                throw new Hata("E-Mail çok uzun");

            var kullanıcı2 = _kullanıcıServisi.KullanıcıAlEmail(yeniEmail);
            if (kullanıcı2 != null && kullanıcı.Id != kullanıcı2.Id)
                throw new Hata("E-Mail adresi zaten eklendi");

            if (doğrulamaGerekli)
            {
                //e-mail yeniden doğrula
                kullanıcı.EmailDoğrulandı = yeniEmail;
                _kullanıcıServisi.KullanıcıGüncelle(kullanıcı);

                //e-mail yeniden doğrulama mesajı
                _genelÖznitelikServisi.ÖznitelikKaydet(kullanıcı, SistemKullanıcıÖznitelikAdları.EmailDoğrulamaKodu, Guid.NewGuid().ToString());
                //_workflowMessageService.SendCustomerEmailRevalidationMessage(kullanıcı, _workContext.WorkingLanguage.Id);
            }
            else
            {
                kullanıcı.Email = yeniEmail;
                _kullanıcıServisi.KullanıcıGüncelle(kullanıcı);

                //abonelik güncelle
                /*
                if (!String.IsNullOrEmpty(eskiEmail) && !eskiEmail.Equals(yeniEmail, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var site in _siteServisi.TümSiteler())
                    {
                        var eskiAbonelik = _bültenAbonelikServisi.BültenAboneliğiAlEmailSiteId(eskiEmail, site.Id);
                        if (eskiAbonelik != null)
                        {
                            eskiAbonelik.Email = yeniEmail;
                            _bültenAbonelikServisi.BültenAboneliğiGüncelle(eskiAbonelik);
                        }
                    }
                }
                */
            }
        }
        public virtual void KullanıcıAdıAyarla(Kullanıcı kullanıcı, string yeniKullanıcıAdı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            if (!_kullanıcıAyarları.KullanıcıAdlarıEtkin)
                throw new Hata("Kullanıcı adı kullanımı kapalı");

            yeniKullanıcıAdı = yeniKullanıcıAdı.Trim();

            if (yeniKullanıcıAdı.Length > 100)
                throw new Hata("Yeni kullanıcı adı çok uzun");

            var kullanıcı2 = _kullanıcıServisi.KullanıcıAlKullanıcıAdı(yeniKullanıcıAdı);
            if (kullanıcı2 != null && kullanıcı.Id != kullanıcı2.Id)
                throw new Hata("Kullanıcı adı zaten mevcut");

            kullanıcı.KullanıcıAdı = yeniKullanıcıAdı;
            _kullanıcıServisi.KullanıcıGüncelle(kullanıcı);
        }

        #endregion
    }
}
