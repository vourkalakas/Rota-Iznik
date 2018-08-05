using Core.Domain.EkTanımlamalar;
using System.Collections.Generic;

namespace Services.EkTanımlamalar
{
    public partial interface IHariciSektorServisi
    {
        void HariciSektorSil(HariciSektor HariciSektor);
        HariciSektor HariciSektorAlId(int HariciSektorId);
        IList<HariciSektor> TümHariciSektorleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void HariciSektorEkle(HariciSektor HariciSektor);
        void HariciSektorGüncelle(HariciSektor HariciSektor);
    }
}
