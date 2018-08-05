using Core;
using Core.Domain.Kongre;
using System.Collections.Generic;

namespace Services.Kongre
{
    public partial interface IKongreServisi
    {
        void KongrelerSil(Kongreler kongreler);
        Kongreler KongrelerAlId(int kongrelerId);
        IList<Kongreler> TümKongrelerAl();
        ISayfalıListe<Kongreler> TümKongrelerAl(int pageIndex = 0, int pageSize = int.MaxValue);
        void KongrelerEkle(Kongreler kongreler);
        void KongrelerGüncelle(Kongreler kongreler);
    }
}
