using Core;
using Core.Domain.Katalog;
using System.Collections.Generic;

namespace Services.Katalog
{
    public partial interface IKategoriServisi
    {
        void KategoriSil(Kategori kategori);
        IList<Kategori> TümKategorileriAlParentKategoriId(int parentKategoriId,
            bool GizliOlanlarıGöster = false, bool tümSeviyeleriDahilEt = false);
        ISayfalıListe<Kategori> TümKategorileriAl(string kategoriAdı = "", int SiteId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool GizliOlanlarıGöster = false);
    }
}