using Core.Domain.Teklif;
using System.Collections.Generic;

namespace Services.Teklifler
{
    public partial interface IBagliTeklifOgesiServisi
    {
        void BagliTeklifOgesiSil(BagliTeklifOgesi bagliTeklifOgesi);
        BagliTeklifOgesi BagliTeklifOgesiAlId(int bagliTeklifOgesiId);
        IList<BagliTeklifOgesi> BagliTeklifOgesiAlTeklifId(int teklifId);
        IList<BagliTeklifOgesi> BagliTeklifOgesiAlTeklifId(int teklifId,string durumu);
        IList<BagliTeklifOgesi> TümBagliTeklifOgesiAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void BagliTeklifOgesiEkle(BagliTeklifOgesi bagliTeklifOgesi);
        void BagliTeklifOgesiGüncelle(BagliTeklifOgesi bagliTeklifOgesi);
    }

}
