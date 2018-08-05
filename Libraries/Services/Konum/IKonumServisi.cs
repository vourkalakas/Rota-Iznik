using Core.Domain.Konum;
using System.Collections.Generic;

namespace Services.Konum
{
    public partial interface IKonumServisi
    {
        void UlkeSil(Ulke ulke);
        Ulke UlkeAlId(int ulkeId);
        IList<Ulke> TümUlkeleriAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void UlkeEkle(Ulke ulke);
        void UlkeGüncelle(Ulke ulke);
        void SehirSil(Sehir sehir);
        Sehir SehirAlId(int sehirId);
        IList<Sehir> TümSehirleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void SehirEkle(Sehir sehir);
        void SehirGüncelle(Sehir sehir);
        void IlceSil(Ilce ilce);
        Ilce IlceAlId(int ilceId);
        IList<Ilce> TümIlceAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<Ilce> IlcelerAlSehirId(int sehirid, bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<Sehir> SehirlerAlUlkeId(int ulkeId, bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void IlceEkle(Ilce ilce);
        void IlceGüncelle(Ilce ilce);
    }
}
