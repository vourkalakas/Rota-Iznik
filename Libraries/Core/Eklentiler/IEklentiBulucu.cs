using System.Collections.Generic;
using Core.Domain.Kullanıcılar;

namespace Core.Eklentiler
{
    public interface IEklentiBulucu
    {
        bool SiteyiOnayla(EklentiTanımlayıcı eklentiTanımlayıcı, int siteId);
        bool KullanıcıİçinYetkili(EklentiTanımlayıcı eklentiTanımlayıcı, Kullanıcı kullanıcı);
        IEnumerable<string> EklentiGruplarınıAl();
        IEnumerable<T> EklentileriAl<T>(EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar,
            Kullanıcı kullanıcı = null, int siteId = 0, string grup = null) where T : class, IEklenti;
        IEnumerable<EklentiTanımlayıcı> EklentiTanımlayıcıAl(EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar,
            Kullanıcı kullanıcı = null, int siteId = 0, string grup = null);
        IEnumerable<EklentiTanımlayıcı> EklentiTanımlayıcıAl<T>(EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar,
            Kullanıcı kullanıcı = null, int siteId = 0, string grup = null) where T : class, IEklenti;
        EklentiTanımlayıcı EklentiTanımlayıcıAlSistemAdı(string sistemAdı, EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar);
        EklentiTanımlayıcı EklentiTanımlayıcıAlSistemAdı<T>(string systemName, EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar)
            where T : class, IEklenti;
        void EklentileriYenidenYükle();
    }
}
