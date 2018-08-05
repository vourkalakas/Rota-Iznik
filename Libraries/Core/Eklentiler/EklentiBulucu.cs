using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Kullanıcılar;

namespace Core.Eklentiler
{
    public class EklentiBulucu : IEklentiBulucu
    {
        #region Fields

        private IList<EklentiTanımlayıcı> _eklentiler;
        private bool _eklentilerYüklendi;

        #endregion

        #region Utilities
        protected virtual void EklentilerinYüklendiğiniKontrolEt()
        {
            if (!_eklentilerYüklendi)
            {
                var bulunanEklentiler = EklentiYönetici.ReferenslıEklentiler.ToList();
                bulunanEklentiler.Sort();
                _eklentiler = bulunanEklentiler.ToList();

                _eklentilerYüklendi = true;
            }
        }
        protected virtual bool YüklemeModunuKontrolEt(EklentiTanımlayıcı eklentiTanımlayıcı, EklentiModuYükle yüklemeModu)
        {
            if (eklentiTanımlayıcı == null)
                throw new ArgumentNullException("eklentiTanımlayıcı");

            switch (yüklemeModu)
            {
                case EklentiModuYükle.Tümü:
                    //filtresiz
                    return true;
                case EklentiModuYükle.Kurulanlar:
                    return eklentiTanımlayıcı.Kuruldu;
                case EklentiModuYükle.Kurulmayanlar:
                    return !eklentiTanımlayıcı.Kuruldu;
                default:
                    throw new Exception("Desteklenmeyen yükleme modu");
            }
        }
        protected virtual bool GrupKontrolEt(EklentiTanımlayıcı eklentiTanımlayıcı, string grup)
        {
            if (eklentiTanımlayıcı == null)
                throw new ArgumentNullException("eklentiTanımlayıcı");

            if (String.IsNullOrEmpty(grup))
                return true;

            return grup.Equals(eklentiTanımlayıcı.Grup, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Methods
        public virtual bool SiteyiOnayla(EklentiTanımlayıcı eklentiTanımlayıcı, int siteId)
        {
            if (eklentiTanımlayıcı == null)
                throw new ArgumentNullException("eklentiTanımlayıcı");

            //doğrulama gerekmiyor
            if (siteId == 0)
                return true;
            if (!eklentiTanımlayıcı.KısıtlıSiteler.Any())
                return true;

            return eklentiTanımlayıcı.KısıtlıSiteler.Contains(siteId);
        }
        public virtual bool KullanıcıİçinYetkili(EklentiTanımlayıcı eklentiTanımlayıcı, Kullanıcı kullanıcı)
        {
            if (eklentiTanımlayıcı == null)
                throw new ArgumentNullException("eklentiTanımlayıcı");

            if (kullanıcı == null || !eklentiTanımlayıcı.KısıtlıMüsteriRolleriListesi.Any())
                return true;

            var kullanıcıRolIdleri = kullanıcı.KullanıcıRolleri.Where(role => role.Aktif).Select(role => role.Id);

            return eklentiTanımlayıcı.KısıtlıMüsteriRolleriListesi.Intersect(kullanıcıRolIdleri).Any();
        }
        public virtual IEnumerable<string> EklentiGruplarınıAl()
        {
            return EklentiTanımlayıcıAl(EklentiModuYükle.Tümü).Select(x => x.Grup).Distinct().OrderBy(x => x);
        }
        public virtual IEnumerable<T> EklentileriAl<T>(EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar,
            Kullanıcı kullanıcı = null, int siteId = 0, string grup = null) where T : class, IEklenti
        {
            return EklentiTanımlayıcıAl<T>(yüklemeModu, kullanıcı, siteId, grup).Select(p => p.Instance<T>());
        }
        public virtual IEnumerable<EklentiTanımlayıcı> EklentiTanımlayıcıAl(EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar,
            Kullanıcı kullanıcı = null, int siteId = 0, string grup = null)
        {
            EklentilerinYüklendiğiniKontrolEt();

            return _eklentiler.Where(p => YüklemeModunuKontrolEt(p, yüklemeModu) && KullanıcıİçinYetkili(p, kullanıcı) && SiteyiOnayla(p, siteId) && GrupKontrolEt(p, grup));
        }
        public virtual IEnumerable<EklentiTanımlayıcı> EklentiTanımlayıcıAl<T>(EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar,
            Kullanıcı kullanıcı = null, int siteId = 0, string grup = null)
            where T : class, IEklenti
        {
            return EklentiTanımlayıcıAl(yüklemeModu, kullanıcı, siteId, grup)
                .Where(p => typeof(T).IsAssignableFrom(p.EklentiTipi));
        }
        public virtual EklentiTanımlayıcı EklentiTanımlayıcıAlSistemAdı(string sistemAdı, EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar)
        {
            return EklentiTanımlayıcıAl(yüklemeModu)
                .SingleOrDefault(p => p.SistemAdı.Equals(sistemAdı, StringComparison.InvariantCultureIgnoreCase));
        }
        public virtual EklentiTanımlayıcı EklentiTanımlayıcıAlSistemAdı<T>(string sistemAdı, EklentiModuYükle yüklemeModu = EklentiModuYükle.Kurulanlar)
            where T : class, IEklenti
        {
            return EklentiTanımlayıcıAl<T>(yüklemeModu)
                .SingleOrDefault(p => p.SistemAdı.Equals(sistemAdı, StringComparison.InvariantCultureIgnoreCase));
        }
        public virtual void EklentileriYenidenYükle()
        {
            _eklentilerYüklendi = false;
            EklentilerinYüklendiğiniKontrolEt();
        }

        #endregion
    }
}
