using Core;
using Core.Domain.Mesajlar;
using System;
using System.Collections.Generic;

namespace Services.Mesajlar
{
    public partial interface IBekleyenMailServisi
    {
        void BekleyenMailEkle(BekleyenMail bekleyenMail);
        void BekleyenMailGüncelle(BekleyenMail bekleyenMail);
        void BekleyenMailSil(BekleyenMail bekleyenMail);
        void BekleyenMailleriSil(IList<BekleyenMail> bekleyenMailler);
        BekleyenMail BekleyenMailAlId(int bekleyenMailId);
        IList<BekleyenMail> BekleyenMailleriAlId(int[] bekleyenMailIds);
        ISayfalıListe<BekleyenMail> EmailleriAra(string emailden,
            string emaile, DateTime? oluşturulmaTarihinden, DateTime? oluşturulmaTarihine,
            bool gönderilmemişÖğeler, bool gönderilmişÖğeler, int maksDenemeSüresi,
            bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);

        void TümEmailleriSil();
    }
}