using Core;
using Core.Domain.Teklif;
using System;
using System.Collections.Generic;

namespace Services.Teklifler
{
    public partial interface ITeklifHariciServisi
    {
        void TeklifSil(TeklifHarici TeklifHarici);
        TeklifHarici TeklifAlId(int TeklifId);
        IList<TeklifHarici> TümTeklifAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void TeklifEkle(TeklifHarici TeklifHarici);
        void TeklifGüncelle(TeklifHarici TeklifHarici);
        ISayfalıListe<TeklifHarici> TeklifAra(string adı, string acenta,string po, string talepno,
           DateTime? tarihi, DateTime? teslimTarihi, bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
    }
}
