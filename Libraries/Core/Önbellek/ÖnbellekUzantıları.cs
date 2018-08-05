using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Önbellek
{
    public static class ÖnbellekUzantıları
    {
        public static T Al<T>(this IÖnbellekYönetici önbellekYönetici, string key, Func<T> kazanım)
        {
            return Al(önbellekYönetici, key, 60, kazanım);
        }
        public static T Al<T>(this IÖnbellekYönetici önbellekYönetici, string key, int önbellekZamanı, Func<T> kazanım)
        {
            if (önbellekYönetici.Ayarlandı(key))
            {
                return önbellekYönetici.Al<T>(key);
            }

            var sonuç = kazanım();
            if (önbellekZamanı > 0)
                önbellekYönetici.Ayarla(key, sonuç, önbellekZamanı);
            return sonuç;
        }
        public static void KalıpİleSil(this IÖnbellekYönetici önbellekYönetici, string kalıp, IEnumerable<string> keys)
        {
            var regex = new Regex(kalıp, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var key in keys.Where(p => regex.IsMatch(p.ToString())).ToList())
                önbellekYönetici.Sil(key);
        }
    }
}
