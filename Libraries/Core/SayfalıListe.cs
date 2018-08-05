using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class SayfalıListe<T> : List<T>, ISayfalıListe<T>
    {
        public SayfalıListe(IQueryable<T> kaynak, int sayfaIndeksi, int sayfaBüyüklüğü)
        {
            int total = kaynak.Count();
            this.TotalCount = total;
            this.TotalPages = total / sayfaBüyüklüğü;

            if (total % sayfaBüyüklüğü > 0)
                TotalPages++;

            this.PageSize = sayfaBüyüklüğü;
            this.PageIndex = sayfaIndeksi;
            this.AddRange(kaynak.Skip(sayfaIndeksi * sayfaBüyüklüğü).Take(sayfaBüyüklüğü).ToList());
        }
        public SayfalıListe(IList<T> kaynak, int sayfaIndeksi, int sayfaBüyüklüğü)
        {
            TotalCount = kaynak.Count();
            TotalPages = TotalCount / sayfaBüyüklüğü;

            if (TotalCount % sayfaBüyüklüğü > 0)
                TotalPages++;

            this.PageSize = sayfaBüyüklüğü;
            this.PageIndex = sayfaIndeksi;
            this.AddRange(kaynak.Skip(sayfaIndeksi * sayfaBüyüklüğü).Take(sayfaBüyüklüğü).ToList());
        }
        public SayfalıListe(IEnumerable<T> kaynak, int sayfaIndeksi, int sayfaBüyüklüğü, int toplamSayı)
        {
            TotalCount = toplamSayı;
            TotalPages = TotalCount / sayfaBüyüklüğü;

            if (TotalCount % sayfaBüyüklüğü > 0)
                TotalPages++;

            this.PageSize = sayfaBüyüklüğü;
            this.PageIndex = sayfaIndeksi;
            this.AddRange(kaynak);
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
