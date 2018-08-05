using Core.Domain.Seo;
using Core.Domain.Siteler;
using System;
using System.Collections.Generic;

namespace Core.Domain.Haber
{
    public partial class HaberÖğesi : TemelVarlık, ISlugDestekli, ISiteMappingDestekli
    {
        private ICollection<HaberYorumu> _haberYorumu;
        public string Başlık { get; set; }
        public string Kısa { get; set; }
        public string Tam { get; set; }
        public bool Yayınlandı { get; set; }
        public DateTime? BaşlangıçTarihi { get; set; }
        public DateTime? BitişTarihi { get; set; }
        public bool Yorumİzinli { get; set; }
        public bool SitelerdeSınırlı { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public virtual ICollection<HaberYorumu> HaberYorumu
        {
            get { return _haberYorumu ?? (_haberYorumu = new List<HaberYorumu>()); }
            protected set { _haberYorumu = value; }
        }
    }
}