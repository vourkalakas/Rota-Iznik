using System;
using System.Collections.Generic;
using Core.Domain.Blogs;
using Core.Domain.Forum;
using Core.Domain.Haber;
using Core.Domain.Kullanıcılar;
using Core.Domain.Mesajlar;
using Core;
using System.Linq;
using Services.Kullanıcılar;
using Services.Siteler;
using Core.Domain.Genel;
using System.Web;
using System.Net;

namespace Services.Mesajlar
{
    public partial class MesajServisi : IMesajServisi
    {
        private readonly ISiteContext _siteContext;
        private readonly IMesajTemasıServisi _mesajTemasıServisi;
        private readonly IEmailHesapServisi _emailHesapServisi;
        private readonly EmailHesapAyarları _emailHesapAyarları;
        private readonly ISiteServisi _siteServisi;
        private readonly GenelAyarlar _genelAyarlar;
        private readonly IBekleyenMailServisi _bekleyenMailServisi;
        public MesajServisi(ISiteContext siteContext,
            IMesajTemasıServisi mesajTemasıServisi,
            IEmailHesapServisi emailHesapServisi,
            EmailHesapAyarları emailHesapAyarları,
            ISiteServisi siteServisi,
            GenelAyarlar genelAyarlar,
            IBekleyenMailServisi bekleyeneMailServisi)
        {
            this._siteContext = siteContext;
            this._mesajTemasıServisi = mesajTemasıServisi;
            this._emailHesapServisi = emailHesapServisi;
            this._emailHesapAyarları = emailHesapAyarları;
            this._siteServisi = siteServisi;
            this._genelAyarlar = genelAyarlar;
            this._bekleyenMailServisi = bekleyeneMailServisi;
        }
        protected virtual MesajTeması AktifMesajTemasınıAl(string mesajTemasıAdı, int siteId)
        {
            var mesajTeması = _mesajTemasıServisi.MesajTemasıAlAdı(mesajTemasıAdı, siteId);

            //tema bulunamadı
            if (mesajTeması == null)
                return null;

            //ensure it's active
            var aktif = mesajTeması.Aktif;
            if (!aktif)
                return null;

            return mesajTeması;
        }
        protected virtual EmailHesabı MesajTemasınınEmailHesabı(MesajTeması mesajTeması)
        {
            var emailHesapId = mesajTeması.EmailHesapId;

            var emailHesabı = _emailHesapServisi.EmailHesabıAlId(emailHesapId);
            if (emailHesabı == null)
                emailHesabı = _emailHesapServisi.EmailHesabıAlId(_emailHesapAyarları.VarsayılanEmailHesapId);
            if (emailHesabı == null)
                emailHesabı = _emailHesapServisi.TümEmailHesaplarıAl().FirstOrDefault();
            return emailHesabı;
        }
        #region Bülten işakışı
        public virtual int BültenAboneliğiAktivasyonMesajıGönder(BültenAboneliği abonelik)
        {
            if (abonelik == null)
                throw new ArgumentNullException("Abonelik");
            var site = _siteContext.MevcutSite;
            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.BültenAboneliğiAktivasyonMesajı, site.Id);
            if (mesajTeması == null)
                return 0;
            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);
            //tokenlar
            
            var tokens = new List<Token>();
            /*_mesajTokenSağlayıcı.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);
            */
            return BildirimGönder(mesajTeması, emailHesabı, tokens, abonelik.Email, string.Empty);
        }

        public virtual int BültenAboneliğiÇıkışıMesajıGönder(BültenAboneliği abonelik)
        {
            if (abonelik == null)
                throw new ArgumentNullException("Abonelik");
            var site = _siteContext.MevcutSite;
            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.BültenAboneliğiAyrılmaMesajı, site.Id);
            if (mesajTeması == null)
                return 0;
            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);
            //tokenlar

            var tokens = new List<Token>();
            /*_mesajTokenSağlayıcı.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);
            */
            return BildirimGönder(mesajTeması, emailHesabı, tokens, abonelik.Email, string.Empty);
        }
        #endregion

        #region Kullanıcı işakışı
        public virtual int KullanıcıyaEmailDoğrulamaMesajıGönder(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.KullanıcıEmailDoğrulamaMesajı, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = kullanıcı.Email;
            var isme = kullanıcı.TamAdAl();
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }

        public virtual int KullanıcıyaEmailDoğrulamaMesajınıYenidenGönder(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.KullanıcıEmailYenidenDoğrulamaMesajı, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = kullanıcı.Email;
            var isme = kullanıcı.TamAdAl();
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }

        public virtual int KullanıcıyaHoşgeldinizMesajıGönder(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.KullanıcıHoşgeldinizMesajı, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = kullanıcı.Email;
            var isme = kullanıcı.TamAdAl();
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }

        public virtual int KullanıcıyaKayıtOlduMesajıGönder(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.KullanıcıKayıtOlduBildirimi, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = emailHesabı.Email;
            var isme = emailHesabı.GörüntülenenAd;
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }

        public virtual int KullanıcıyaŞifreKurtarmaMesajıGönder(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.KullanıcıŞifreKurtarmaMesajı, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = kullanıcı.Email;
            var isme = kullanıcı.TamAdAl();
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }
        #endregion

        #region Forum Bildirimleri
        public virtual int YeniForumGirdisiMesajıGönder(Kullanıcı kullanıcı, ForumGirdisi forumGirdisi, ForumSayfası forumSayfası, Forum forum, int ForumSayfasıSayfaIndexi)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.YeniForumGirdiMesajı, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
            _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = kullanıcı.Email;
            var isme = kullanıcı.TamAdAl();
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }

        public virtual int YeniForumSayfasıMesajıGönder(Kullanıcı kullanıcı, ForumSayfası forumSayfası, Forum forum)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.YeniForumSayfasıMesajı, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
            _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = kullanıcı.Email;
            var isme = kullanıcı.TamAdAl();
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }

        public virtual int ÖzelMesajBildirimiGönder(ÖzelMesaj özelMesaj)
        {
            if (özelMesaj == null)
                throw new ArgumentNullException("özelMesaj");

            var site = _siteServisi.SiteAlId(özelMesaj.SiteId) ?? _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.ÖzelMesajBildirimi, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
            _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _olayYayınlayıcı.MessageTokensAdded(mesajTeması, tokens);
            */
            var Emaile = özelMesaj.Kullanıcıya.Email;
            var isme = özelMesaj.Kullanıcıya.TamAdAl();
            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }
        #endregion

        #region Diğer
        public virtual int HaberYorumuBildirimMesajıGönder(HaberYorumu haberYorumu)
        {
            if (haberYorumu == null)
                throw new ArgumentNullException("haberYorumu");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.HaberYorumuBildirimi, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);
            _messageTokenProvider.AddCustomerTokens(tokens, blogComment.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
            */
            var Emaile = emailHesabı.Email;
            var isme = emailHesabı.GörüntülenenAd;

            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }
        public virtual int BlogYorumuBildirimMesajıGönder(BlogYorumu blogYorumu)
        {
            if (blogYorumu == null)
                throw new ArgumentNullException("blogYorumu");

            var site = _siteContext.MevcutSite;

            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.BlogYorumuBildirimi, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            //tokenlar
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);
            _messageTokenProvider.AddCustomerTokens(tokens, blogComment.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
            */
            var Emaile = emailHesabı.Email;
            var isme = emailHesabı.GörüntülenenAd;

            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isme);
        }
        public virtual int BildirimGönder(MesajTeması mesajTeması, EmailHesabı emailHesabı, IEnumerable<Token> tokens, string emailAdresine, string isime, string ekDosyaYolu = null, string ekDosyaAdı = null, string emailAdresineCevap = null, string ismeCevap = null, string emailAdresinden = null, string isimden = null, string konu = null)
        {
            if (mesajTeması == null)
                throw new ArgumentNullException("mesajTeması");

            if (emailHesabı == null)
                throw new ArgumentNullException("emailHesabı");

            var bcc = mesajTeması.BccEmailAdresleri;
            if (String.IsNullOrEmpty(konu))
                konu = mesajTeması.Konu;
            var gövde = mesajTeması.Gövde;

            //Replace subject and body tokens 
            /*
            var subjectReplaced = _tokenizer.Replace(konu, tokens, false);
            var bodyReplaced = _tokenizer.Replace(gövde, tokens, true);
            */
            //limit name length
            isime = GenelYardımcı.MaksimumUzunlukKontrol(isime, 300);

            var email = new BekleyenMail
            {
                Öncelik = BekleyenMailÖnceliği.Yüksek,
                Kimden = !string.IsNullOrEmpty(emailAdresinden) ? emailAdresinden : emailHesabı.Email,
                KimdenAd = !string.IsNullOrEmpty(isimden) ? isimden : emailHesabı.GörüntülenenAd,
                Kime = emailAdresine,
                KimeAd = isime,
                Yanıtla = emailAdresineCevap,
                YanıtlaAd = ismeCevap,
                CC = string.Empty,
                Bcc = bcc,
                Konu = konu,//tokenizer
                Gövde = gövde,//tokenizer
                EkDosyaYolu = ekDosyaYolu,
                EkDosyaAdı = ekDosyaAdı,
                EkYüklemeId = mesajTeması.EkİndirmeId,
                OluşturulmaTarihi = DateTime.UtcNow,
                EmailHesapId = emailHesabı.Id,
                ŞuTarihdenÖnceGönderme = !mesajTeması.GöndermedenÖnceGeciktir.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(mesajTeması.GecikmePeriodu.Saatler(mesajTeması.GöndermedenÖnceGeciktir.Value)))
            };

            _bekleyenMailServisi.BekleyenMailEkle(email);
            return email.Id;
        }

        public virtual int BizimleİletişimeGeçinMesajıGönder(string gönderenMaili, string gönderenAdı, string konu, string gövde)
        {
            var site = _siteContext.MevcutSite;
            var mesajTeması = AktifMesajTemasınıAl(MesajTemasıSistemAdları.İletişimeGeçinMesajı, site.Id);
            if (mesajTeması == null)
                return 0;

            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            string Emailden;
            string isimden;
            //required for some SMTP servers
            if (_genelAyarlar.İletişimFormuSistemMaili)
            {
                Emailden = emailHesabı.Email;
                isimden = emailHesabı.GörüntülenenAd;
                gövde = $"<strong>From</strong>: {WebUtility.HtmlEncode(gönderenAdı)} - {WebUtility.HtmlEncode(gönderenMaili)}<br /><br />{gövde}";
            }
            else
            {
                Emailden = gönderenMaili;
                isimden = gönderenAdı;
            }

            //tokens
            var tokens = new List<Token>();
            /*
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            tokens.Add(new Token("ContactUs.SenderEmail", senderEmail));
            tokens.Add(new Token("ContactUs.SenderName", senderName));
            tokens.Add(new Token("ContactUs.Body", body, true));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
            */
            var Emaile = emailHesabı.Email;
            var isime = emailHesabı.GörüntülenenAd;

            return BildirimGönder(mesajTeması, emailHesabı, tokens, Emaile, isime,
                emailAdresinden: Emailden,
                isimden: isimden,
                konu: konu,
                emailAdresineCevap: gönderenMaili,
                ismeCevap: gönderenAdı);
        }
        public virtual int TestEmailiGönder(int messageTemplateId, string sendToEmail, List<Token> tokens)
        {
            var mesajTeması = _mesajTemasıServisi.MesajTemasıAlId(messageTemplateId);
            if (mesajTeması == null)
                throw new ArgumentException("Tema yüklenemedi");

            //email hesabı
            var emailHesabı = MesajTemasınınEmailHesabı(mesajTeması);

            return BildirimGönder(mesajTeması, emailHesabı, tokens, sendToEmail, null);
        }
        #endregion
    }
}
