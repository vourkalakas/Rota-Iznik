using Core.Domain.Mesajlar;
using System.Collections.Generic;

namespace Services.Mesajlar
{
    public partial interface IEmailHesapServisi
    {
        void EmailHesabıEkle(EmailHesabı emailHesabı);
        void EmailHesabıSil(EmailHesabı emailHesabı);
        void EmailHesabıGüncelle(EmailHesabı emailHesabı);
        EmailHesabı EmailHesabıAlId(int emailHesapId);
        IList<EmailHesabı> TümEmailHesaplarıAl();

    }
}
