using Core.Domain.Katalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Katalog
{
    public static class KategoriUzantıları
    {
        public static IList<Kategori> KategorileriAğaçİçinSırala(this IList<Kategori> kaynak, int parentId = 0, bool parentiOlmayanKategorileriYoksay = false)
        {
            if (kaynak == null)
                throw new ArgumentNullException("kaynak");

            var sonuç = new List<Kategori>();

            foreach (var cat in kaynak.Where(c => c.ParentKategoriId == parentId).ToList())
            {
                sonuç.Add(cat);
                sonuç.AddRange(KategorileriAğaçİçinSırala(kaynak, cat.Id, true));
            }
            if (!parentiOlmayanKategorileriYoksay && sonuç.Count != kaynak.Count)
            {
                foreach (var cat in kaynak)
                    if (sonuç.FirstOrDefault(x => x.Id == cat.Id) == null)
                        sonuç.Add(cat);
            }
            return sonuç;
        }
    }
}
