using Core.Domain.Sayfalar;
using System.Collections.Generic;

namespace Services.Sayfalar
{
    public partial interface ISayfalarServisi
    {
        void SayfaSil(Sayfa sayfa);
        Sayfa SayfaAlId(int sayfaId);
        Sayfa SayfaAlSistemAdı(string systemName, int storeId = 0);
        IList<Sayfa> TümSayfalarıAl(int siteId, bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void SayfaEkle(Sayfa sayfa);
        void SayfaGüncelle(Sayfa sayfa);
    }
}
