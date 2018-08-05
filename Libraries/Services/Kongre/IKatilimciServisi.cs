using Core;
using Core.Domain.Kongre;
using System.Collections.Generic;

namespace Services.Kongre
{
    public partial interface IKatilimciServisi
    {
        void KatilimciSil(Katilimci katilimci);
        Katilimci KatilimciAlId(int katilimciId);
        IList<Katilimci> TümKatilimciAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void KatilimciEkle(Katilimci katilimci);
        void KatilimciGüncelle(Katilimci katilimci);
        ISayfalıListe<Katilimci> KatılımcıAra(int kongreId, string katılımcıAdı,
           bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
    }

}
