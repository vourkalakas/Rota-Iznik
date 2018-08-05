using Core.Domain.Kongre;
using System.Collections.Generic;
namespace Services.Kongre
{
    public partial interface IKonaklamaServisi
    {
        void KonaklamaSil(Konaklama konaklama);
        Konaklama KonaklamaAlId(int konaklamaId);
        IList<Konaklama> TümKonaklamaAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<Konaklama> KonaklamaAlKongreId(int kongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void KonaklamaEkle(Konaklama konaklama);
        void KonaklamaGüncelle(Konaklama konaklama);
    }

}
