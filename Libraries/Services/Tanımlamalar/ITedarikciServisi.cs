using Core.Domain.Tanımlamalar;
using System.Collections.Generic;

namespace Services.Tanımlamalar
{
    public partial interface ITedarikciServisi
    {
        void TedarikciSil(Tedarikci tedarikci);
        Tedarikci TedarikciAlId(int tedarikciId);
        IList<Tedarikci> TümTedarikciAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void TedarikciEkle(Tedarikci tedarikci);
        void TedarikciGüncelle(Tedarikci tedarikci);
    }

}
