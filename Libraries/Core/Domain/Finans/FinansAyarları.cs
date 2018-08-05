using Core.Domain.Kullanıcılar;
using Core.Yapılandırma;
using System.Collections.Generic;

namespace Core.Domain.Finans
{
    public class FinansAyarları:IAyarlar
    {
        public IList<KullanıcıRolü> BildirimRolleri { get; set; }
    }
}
