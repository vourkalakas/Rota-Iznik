using Core.Domain.Mesajlar;
using System.Collections.Generic;

namespace Services.Mesajlar
{
    public partial interface IEmailGönderici
    {

        void SendEmail(EmailHesabı emailHesabı, string konu, string gövde,
                string adresinden, string isminden, string adresine, string ismine,
                 string cevapAdresi = null, string cevapAdı = null,
                IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
                string ekDosyaYolu = null, string ekDosyaAdı = null,
                int ekİndirmeId = 0, IDictionary<string, string> basşlıklar = null);
    }
}
