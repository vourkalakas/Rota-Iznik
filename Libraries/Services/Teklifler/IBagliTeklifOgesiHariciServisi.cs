using Core.Domain.Teklif;
using System.Collections.Generic;

namespace Services.Teklifler
{
    public partial interface IBagliTeklifOgesiHariciServisi
    {
        void BagliTeklifOgesiSil(BagliTeklifOgesiHarici bagliTeklifOgesi);
        BagliTeklifOgesiHarici BagliTeklifOgesiAlId(int bagliTeklifOgesiId);
        IList<BagliTeklifOgesiHarici> BagliTeklifOgesiAlTeklifId(int teklifId);
        IList<BagliTeklifOgesiHarici> BagliTeklifOgesiAlTeklifId(int teklifId,string durumu);
        IList<BagliTeklifOgesiHarici> TümBagliTeklifOgesiAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void BagliTeklifOgesiEkle(BagliTeklifOgesiHarici bagliTeklifOgesi);
        void BagliTeklifOgesiGüncelle(BagliTeklifOgesiHarici bagliTeklifOgesi);
    }

}
