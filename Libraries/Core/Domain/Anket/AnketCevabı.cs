using System.Collections.Generic;

namespace Core.Domain.Anket
{
    public partial class AnketCevabı : TemelVarlık
    {
        private ICollection<AnketOyKaydı> _anketOyKaydı;

        public int AnketId { get; set; }

        public string Adı { get; set; }

        public int OySayısı { get; set; }

        public int GörüntülenmeSırası { get; set; }

        public virtual Anket Anket { get; set; }

        public virtual ICollection<AnketOyKaydı> AnketOyKaydı
        {
            get { return _anketOyKaydı ?? (_anketOyKaydı = new List<AnketOyKaydı>()); }
            protected set { _anketOyKaydı = value; }
        }
    }
}
