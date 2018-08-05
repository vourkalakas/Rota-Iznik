using Core.Domain.Kongre;
using System.Collections.Generic;

namespace Services.Kongre
{
    public partial interface IKayitServisi
    {
        void KayitSil(Kayit kayit);
        Kayit KayitAlId(int kayitId);
        IList<Kayit> TümKayitAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<Kayit> KayitAlKongreId(int kongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void KayitEkle(Kayit kayit);
        void KayitGüncelle(Kayit kayit);
    }

}
