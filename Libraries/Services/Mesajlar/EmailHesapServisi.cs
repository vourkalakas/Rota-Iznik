using System;
using System.Collections.Generic;
using Core.Domain.Mesajlar;
using Core.Data;
using Services.Olaylar;
using Core;
using System.Linq;

namespace Services.Mesajlar
{
    public partial class EmailHesapServisi : IEmailHesapServisi
    {
        private readonly IDepo<EmailHesabı> _emailDepo;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        public EmailHesapServisi(IDepo<EmailHesabı> emailDepo,
                                IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._emailDepo = emailDepo;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }
        
        public virtual void EmailHesabıGüncelle(EmailHesabı emailHesabı)
        {
            if (emailHesabı == null)
                throw new ArgumentNullException("emailHesabı");

            emailHesabı.Email = GenelYardımcı.BoşKontrol(emailHesabı.Email);
            emailHesabı.GörüntülenenAd = GenelYardımcı.BoşKontrol(emailHesabı.GörüntülenenAd);
            emailHesabı.Host = GenelYardımcı.BoşKontrol(emailHesabı.Host);
            emailHesabı.KullanıcıAdı = GenelYardımcı.BoşKontrol(emailHesabı.KullanıcıAdı);
            emailHesabı.Şifre = GenelYardımcı.BoşKontrol(emailHesabı.Şifre);

            emailHesabı.Email = emailHesabı.Email.Trim();
            emailHesabı.GörüntülenenAd = emailHesabı.GörüntülenenAd.Trim();
            emailHesabı.Host = emailHesabı.Host.Trim();
            emailHesabı.KullanıcıAdı = emailHesabı.KullanıcıAdı.Trim();
            emailHesabı.Şifre = emailHesabı.Şifre.Trim();

            emailHesabı.Email = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.Email, 255);
            emailHesabı.GörüntülenenAd = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.GörüntülenenAd, 255);
            emailHesabı.Host = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.Host, 255);
            emailHesabı.KullanıcıAdı = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.KullanıcıAdı, 255);
            emailHesabı.Şifre = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.Şifre, 255);

            _emailDepo.Güncelle(emailHesabı);
            _olayYayınlayıcı.OlayGüncellendi(emailHesabı);
        }

        public virtual EmailHesabı EmailHesabıAlId(int emailHesapId)
        {
            if (emailHesapId == 0)
                return null;

            return _emailDepo.AlId(emailHesapId);
        }

        public virtual void EmailHesabıEkle(EmailHesabı emailHesabı)
        {
            if (emailHesabı == null)
                throw new ArgumentNullException("emailHesabı");

            emailHesabı.Email = GenelYardımcı.BoşKontrol(emailHesabı.Email);
            emailHesabı.GörüntülenenAd = GenelYardımcı.BoşKontrol(emailHesabı.GörüntülenenAd);
            emailHesabı.Host = GenelYardımcı.BoşKontrol(emailHesabı.Host);
            emailHesabı.KullanıcıAdı = GenelYardımcı.BoşKontrol(emailHesabı.KullanıcıAdı);
            emailHesabı.Şifre = GenelYardımcı.BoşKontrol(emailHesabı.Şifre);

            emailHesabı.Email = emailHesabı.Email.Trim();
            emailHesabı.GörüntülenenAd = emailHesabı.GörüntülenenAd.Trim();
            emailHesabı.Host = emailHesabı.Host.Trim();
            emailHesabı.KullanıcıAdı = emailHesabı.KullanıcıAdı.Trim();
            emailHesabı.Şifre = emailHesabı.Şifre.Trim();

            emailHesabı.Email = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.Email, 255);
            emailHesabı.GörüntülenenAd = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.GörüntülenenAd, 255);
            emailHesabı.Host = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.Host, 255);
            emailHesabı.KullanıcıAdı = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.KullanıcıAdı, 255);
            emailHesabı.Şifre = GenelYardımcı.MaksimumUzunlukKontrol(emailHesabı.Şifre, 255);

            _emailDepo.Ekle(emailHesabı);
            _olayYayınlayıcı.OlayEklendi(emailHesabı);
        }

        public virtual void EmailHesabıSil(EmailHesabı emailHesabı)
        {
            if (emailHesabı == null)
                throw new ArgumentNullException("emailHesabı");
            if (TümEmailHesaplarıAl().Count == 1)
                throw new Hata("Bu E-mail hesabını silemezsiniz.En azından bir E-Mail hesabı bulunmalıdır.");
            _olayYayınlayıcı.OlaySilindi(emailHesabı);
            _emailDepo.Sil(emailHesabı);
        }

        public virtual IList<EmailHesabı> TümEmailHesaplarıAl()
        {
            var sorgu = from ea in _emailDepo.Tablo
                        orderby ea.Id
                        select ea;
            var emailHesapları = sorgu.ToList();
            return emailHesapları;
        }
    }
}
