using Core;
using Core.Domain.Teklif;
using System;
using System.Collections.Generic;

namespace Services.Teklifler
{
    public partial interface ITeklifServisi
    {
        void TeklifSil(Teklif Teklif);
        Teklif TeklifAlId(int TeklifId);
        IList<Teklif> TümTeklifAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void TeklifEkle(Teklif Teklif);
        void TeklifGüncelle(Teklif Teklif);
        ISayfalıListe<Teklif> TeklifAra(DateTime? tarihinden = null,
            DateTime? tarihine = null, int hazırlayanId = 0, string adı = "",
            string Konumu = "", string açıklama = "", string durumu = "",
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
    }
}
