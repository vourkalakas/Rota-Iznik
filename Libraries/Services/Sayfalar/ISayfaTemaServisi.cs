using Core.Domain.Sayfalar;
using System.Collections.Generic;

namespace Services.Sayfalar
{
    public partial interface ISayfaTemaServisi
    {
        void SayfaTemaSil(SayfaTema sayfaTema);
        IList<SayfaTema> TümSayfaTemalar();
        SayfaTema SayfaTemaAlId(int sayfaTemaId);
        void SayfaTemaEkle(SayfaTema sayfaTema);
        void SayfaTemaGüncelle(SayfaTema sayfaTema);
    }
}
