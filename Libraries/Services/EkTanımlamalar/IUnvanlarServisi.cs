using Core.Domain.EkTanımlamalar;
using System.Collections.Generic;

namespace Services.EkTanımlamalar
{
    public partial interface IUnvanlarServisi
    {
        void UnvanlarSil(Unvanlar ünvanlar);
        Unvanlar UnvanlarAlId(int ünvanlarId);
        IList<Unvanlar> TümUnvanlarıAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void UnvanlarEkle(Unvanlar ünvanlar);
        void UnvanlarGüncelle(Unvanlar ünvanlar);
    }
}
