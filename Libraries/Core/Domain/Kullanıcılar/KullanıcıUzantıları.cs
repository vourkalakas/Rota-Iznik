using System;
using System.Linq;
using Core.Domain.Genel;

namespace Core.Domain.Kullanıcılar
{
    public static class KullanıcıUzantıları
    {
        #region Kullanıcı rolü
        public static bool KullanıcıRolünde(this Kullanıcı Kullanıcı,
            string kullanıcıRolüSistemAdı, bool sadeceAktifKullanıcıRolleri = true)
        {
            if (Kullanıcı == null)
                throw new ArgumentNullException("Kullanıcı");

            if (String.IsNullOrEmpty(kullanıcıRolüSistemAdı))
                throw new ArgumentNullException("customerRoleSystemName");

            var sonuç = Kullanıcı.KullanıcıRolleri
                .FirstOrDefault(cr => (!sadeceAktifKullanıcıRolleri || cr.Aktif) && (cr.SistemAdı == kullanıcıRolüSistemAdı)) != null;
            return sonuç;
        }
        public static bool AramaMotoruHesabı(this Kullanıcı Kullanıcı)
        {
            if (Kullanıcı == null)
                throw new ArgumentNullException("Kullanıcı");

            if (!Kullanıcı.SistemHesabı || String.IsNullOrEmpty(Kullanıcı.SistemAdı))
                return false;

            var sonuç = Kullanıcı.SistemAdı.Equals(SistemKullanıcıAdları.AramaMotoru, StringComparison.InvariantCultureIgnoreCase);
            return sonuç;
        }
        public static bool ArkaPlanGöreviHesabı(this Kullanıcı Kullanıcı)
        {
            if (Kullanıcı == null)
                throw new ArgumentNullException("Kullanıcı");

            if (!Kullanıcı.SistemHesabı || String.IsNullOrEmpty(Kullanıcı.SistemAdı))
                return false;

            var sonuç = Kullanıcı.SistemAdı.Equals(SistemKullanıcıAdları.ArkaPlanGörevi, StringComparison.InvariantCultureIgnoreCase);
            return sonuç;
        }
        public static bool Yönetici(this Kullanıcı Kullanıcı, bool sadeceAktifKullanıcıRolleri = true)
        {
            return KullanıcıRolünde(Kullanıcı, SistemKullanıcıRolAdları.Yönetici, sadeceAktifKullanıcıRolleri);
        }
        /*
        public static bool ForumYöneticisi(this Kullanıcı Kullanıcı, bool sadeceAktifKullanıcıRolleri = true)
        {
            return KullanıcıRolünde(Kullanıcı, SistemKullanıcıRolAdları.ForumYönetici, sadeceAktifKullanıcıRolleri);
        }*/
        public static bool IsRegistered(this Kullanıcı Kullanıcı, bool sadeceAktifKullanıcıRolleri = true)
        {
            return KullanıcıRolünde(Kullanıcı, SistemKullanıcıRolAdları.Kayıtlı, sadeceAktifKullanıcıRolleri);
        }
        public static bool IsGuest(this Kullanıcı Kullanıcı, bool sadeceAktifKullanıcıRolleri = true)
        {
            return KullanıcıRolünde(Kullanıcı, SistemKullanıcıRolAdları.Ziyaretçi, sadeceAktifKullanıcıRolleri);
        }
        #endregion

        #region Adresler

        public static void AdresSil(this Kullanıcı Kullanıcı, Adres adres)
        {
            if (Kullanıcı.Adresler.Contains(adres))
            {
                if (Kullanıcı.FaturaAdresi == adres) Kullanıcı.FaturaAdresi = null;

                Kullanıcı.Adresler.Remove(adres);
            }
        }

        #endregion
    }
}
