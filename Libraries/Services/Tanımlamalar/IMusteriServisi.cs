using Core.Domain.Tanımlamalar;
using System.Collections.Generic;

namespace Services.Tanımlamalar
{
    public partial interface IMusteriServisi
    {
        void MusteriSil(Musteri musteri);
        Musteri MusteriAlId(int musteriId);
        IList<Musteri> TümMusteriAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void MusteriEkle(Musteri musteri);
        void MusteriGüncelle(Musteri musteri);
    }
}
