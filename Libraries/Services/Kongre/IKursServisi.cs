using Core.Domain.Kongre;
using System.Collections.Generic;
namespace Services.Kongre
{
    public partial interface IKursServisi
    {
        void KursSil(Kurs kurs);
        Kurs KursAlId(int kursId);
        IList<Kurs> TümKursAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<Kurs> KursAlKongreId(int KongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void KursEkle(Kurs kurs);
        void KursGüncelle(Kurs kurs);
    }

}
