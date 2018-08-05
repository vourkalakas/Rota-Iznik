using Core;
using Core.Domain.Yetkililer;
using System.Collections.Generic;

namespace Services.Yetkililer
{
    public partial interface IYetkiliServisi
    {
        void YetkiliSil(Yetkili yetkililer);
        Yetkili YetkiliAlId(int yetkililerId);
        ISayfalıListe<Yetkili> TümYetkiliAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false, int pageIndex = 0, int pageSize = int.MaxValue);
        IList<Yetkili> YetkiliAlGrup(int grup, int grupId);
        void YetkiliEkle(Yetkili yetkililer);
        void YetkiliGüncelle(Yetkili yetkililer);
    }

}
