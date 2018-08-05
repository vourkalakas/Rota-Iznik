using Core.Domain.Görüşmeler;
using System.Collections.Generic;

namespace Services.Görüşmeler
{
    public partial interface IGorusmeRaporlariServisi
    {
        void GorusmeRaporlariSil(GorusmeRaporlari gorusmeRaporlari);
        GorusmeRaporlari GorusmeRaporlariAlId(int gorusmeRaporlariId);
        IList<GorusmeRaporlari> TümGorusmeRaporlariAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void GorusmeRaporlariEkle(GorusmeRaporlari gorusmeRaporlari);
        void GorusmeRaporlariGüncelle(GorusmeRaporlari gorusmeRaporlari);
    }

}
