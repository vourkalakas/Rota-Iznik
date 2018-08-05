using System;
using System.Collections.Generic;

namespace Core.Domain.Anket
{
    public partial class Anket : TemelVarlık
    {
        private ICollection<AnketCevabı> _anketCevabı;

        public string Adı { get; set; }

        public string SistemAnahtarKelime { get; set; }

        public bool Yayınlandı { get; set; }

        public bool AnasayfadaGöster { get; set; }

        public bool Ziyaretçilerİzinli { get; set; }

        public int GörüntülenmeSırası { get; set; }

        public DateTime? BaşlangıçTarihi { get; set; }

        public DateTime? BitişTarihi { get; set; }

        public virtual ICollection<AnketCevabı> AnketCevabı
        {
            get { return _anketCevabı ?? (_anketCevabı = new List<AnketCevabı>()); }
            protected set { _anketCevabı = value; }
        }

    }
}
