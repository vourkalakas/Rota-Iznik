using Core.Domain.Kongre;
using System.Collections.Generic;

namespace Services.Kongre
{
    public partial interface IRefakatciServisi
    {
        void RefakatciSil(Refakatci refakatci);
        Refakatci RefakatciAlId(int refakatciId);
        IList<Refakatci> TümRefakatciAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void RefakatciEkle(Refakatci refakatci);
        void RefakatciGüncelle(Refakatci refakatci);
    }

}
