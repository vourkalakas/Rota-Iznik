using Core.Domain.EkTanımlamalar;
using System.Collections.Generic;

namespace Services.EkTanımlamalar
{
    public partial interface ITeklifKalemiServisi
    {
        void TeklifKalemiSil(TeklifKalemi TeklifKalemi);
        TeklifKalemi TeklifKalemiAlId(int TeklifKalemiId);
        IList<TeklifKalemi> TümTeklifKalemleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<TeklifKalemi> AnaTeklifKalemleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void TeklifKalemiEkle(TeklifKalemi TeklifKalemi);
        void TeklifKalemiGüncelle(TeklifKalemi TeklifKalemi);
    }
}
