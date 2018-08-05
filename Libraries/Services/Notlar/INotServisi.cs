using Core.Domain.Notlar;
using System.Collections.Generic;

namespace Services.Notlar
{
    public partial interface INotServisi
    {
        void NotSil(Not not);
        Not NotAlId(int notId);
        IList<Not> NotAlId(int userId,string grup,int? grupId);
        IList<Not> TümNotAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void NotEkle(Not not);
        void NotGüncelle(Not not);
    }

}
