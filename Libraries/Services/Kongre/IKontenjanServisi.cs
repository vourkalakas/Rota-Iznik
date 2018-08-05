using Core;
using Core.Domain.Kongre;
using System.Collections.Generic;

namespace Services.Kongre
{
    public partial interface IKontenjanServisi
    {
        void KontenjanSil(Kontenjan Kontenjan);
        Kontenjan KontenjanAlId(int KontenjanId);
        IList<Kontenjan> TümKontenjanAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<Kontenjan> KontenjanAlKongreId(int KongreId);
        void KontenjanEkle(Kontenjan Kontenjan);
        void KontenjanGüncelle(Kontenjan Kontenjan);
    }

}
