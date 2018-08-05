using System;
using Core.Domain.Kullanıcılar;

namespace Services.KimlikDoğrulama.Harici
{
    public static class HariciYetkilendirmeMetoduUzantıları
    {
        public static bool MetodAktif(this IHariciYetkilendirmeMetodu metod, HariciYetkilendirmeAyarları ayarlar)
        {
            if (metod == null)
                throw new ArgumentNullException(nameof(metod));

            if (ayarlar == null)
                throw new ArgumentNullException(nameof(ayarlar));

            if (ayarlar.AktifYetkilendirmeMetoduSistemAdları == null)
                return false;

            foreach (var activeMethodSystemName in ayarlar.AktifYetkilendirmeMetoduSistemAdları)
                if (metod.EklentiTanımlayıcı.SistemAdı.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }
    }
}
