using Core.Domain.Blogs;
using Core.Domain.Forum;
using Core.Domain.Haber;
using Core.Domain.Kullanıcılar;
using Core.Domain.Mesajlar;
using System.Collections.Generic;

namespace Services.Mesajlar
{
    public partial interface IMesajServisi
    {
        #region Kullanıcı işakışı

        int KullanıcıyaKayıtOlduMesajıGönder(Kullanıcı kullanıcı);
        int KullanıcıyaHoşgeldinizMesajıGönder(Kullanıcı kullanıcı);
        int KullanıcıyaEmailDoğrulamaMesajıGönder(Kullanıcı kullanıcı);
        int KullanıcıyaEmailDoğrulamaMesajınıYenidenGönder(Kullanıcı kullanıcı);
        int KullanıcıyaŞifreKurtarmaMesajıGönder(Kullanıcı kullanıcı);

        #endregion

        #region Bülten işakışı
        
        int BültenAboneliğiAktivasyonMesajıGönder(BültenAboneliği abonelik);
        int BültenAboneliğiÇıkışıMesajıGönder(BültenAboneliği abonelik);

        #endregion

        #region Forum Bildirimleri
        int YeniForumSayfasıMesajıGönder(Kullanıcı kullanıcı,ForumSayfası forumSayfası, Forum forum);
        int YeniForumGirdisiMesajıGönder(Kullanıcı kullanıcı,
            ForumGirdisi forumGirdisi, ForumSayfası forumSayfası,
            Forum forum, int ForumSayfasıSayfaIndexi);
        int ÖzelMesajBildirimiGönder(ÖzelMesaj özelMesaj);
        #endregion

        #region Diğer
        //int SendGiftCardNotification(GiftCard giftCard);
        int BlogYorumuBildirimMesajıGönder(BlogYorumu blogYorumu);
        int HaberYorumuBildirimMesajıGönder(HaberYorumu haberYorumu);
        int BizimleİletişimeGeçinMesajıGönder(string gönderenMaili, string gönderenAdı, string konu, string gövde);
        int TestEmailiGönder(int messageTemplateId, string sendToEmail, List<Token> tokens);
        int BildirimGönder(MesajTeması mesajTeması,
            EmailHesabı emailHesabı, IEnumerable<Token> tokens,
            string emailAdresine, string isime,
            string ekDosyaYolu = null, string ekDosyaAdı = null,
            string emailAdresineCevap = null, string ismeCevap = null,
            string emailAdresinden = null, string isimden = null, string konu = null);

        #endregion
    }
}
