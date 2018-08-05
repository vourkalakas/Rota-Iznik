using Core.Domain.EkTanımlamalar;
using System.Collections.Generic;

namespace Services.EkTanımlamalar
{
    public partial interface IMusteriSektorServisi
    {
        void MusteriSil(MusteriSektor musteri);
        MusteriSektor MusteriAlId(int musteriId);
        IList<MusteriSektor> TümMusterilarıAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void MusteriEkle(MusteriSektor musteri);
        void MusteriGüncelle(MusteriSektor musteri);
    }
}
