using Core.Domain.EkTanımlamalar;
using System.Collections.Generic;

namespace Services.EkTanımlamalar
{
    public partial interface ITedarikciSektorServisi
    {
        void TedarikciSektorSil(TedarikciSektor TedarikciSektor);
        TedarikciSektor TedarikciSektorAlId(int TedarikciSektorId);
        IList<TedarikciSektor> TümTedarikciSektorleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void TedarikciSektorEkle(TedarikciSektor TedarikciSektor);
        void TedarikciSektorGüncelle(TedarikciSektor TedarikciSektor);
    }
}
