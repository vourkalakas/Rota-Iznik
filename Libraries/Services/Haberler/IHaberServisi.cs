using Core;
using Core.Domain.Haber;
using System;
using System.Collections.Generic;

namespace Services.Haberler
{
    public partial interface IHaberServisi
    {
        #region Haberler

        void HaberSil(HaberÖğesi haberÖğesi);

        HaberÖğesi HaberAlId(int haberId);

        IList<HaberÖğesi> HaberAlIdler(int[] haberIdleri);

        ISayfalıListe<HaberÖğesi> TümHaberleriAl(int siteId = 0,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false);

        void HaberEkle(HaberÖğesi haberler);

        void HaberGüncelle(HaberÖğesi haberler);

        #endregion

        #region Haber yorumları

        IList<HaberYorumu> TümYorumlarıAl(int kullanıcıId = 0, int siteId = 0, int? HaberÖğesiId = null,
            bool? onaylandı = null, DateTime? tarihinden = null, DateTime? tarihine = null, string yorumYazısı = null);

        HaberYorumu YorumAlId(int haberYorumuId);

        IList<HaberYorumu> YorumAlIdler(int[] yorumIdleri);

        int YorumSayısı(HaberÖğesi HaberÖğesi, int sited = 0, bool? onaylandı = null);

        void yorumSil(HaberYorumu haberYorumu);

        void yorumlarıSil(IList<HaberYorumu> haberYorumları);

        #endregion
    }
}
