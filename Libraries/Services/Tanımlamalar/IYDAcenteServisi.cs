using Core.Domain.Tanımlamalar;
using System.Collections.Generic;

namespace Services.Tanımlamalar
{
    public partial interface IYDAcenteServisi
    {
        void YDAcenteSil(YDAcente yDAcente);
        YDAcente YDAcenteAlId(int yDAcenteId);
        IList<YDAcente> TümYDAcenteAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void YDAcenteEkle(YDAcente yDAcente);
        void YDAcenteGüncelle(YDAcente yDAcente);
    }

}
