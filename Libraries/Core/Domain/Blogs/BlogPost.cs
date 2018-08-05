using Core.Domain.Seo;
using Core.Domain.Siteler;
using System;
using System.Collections.Generic;
namespace Core.Domain.Blogs
{
    public partial class BlogPost : TemelVarlık, ISlugDestekli, ISiteMappingDestekli
    {
        private ICollection<BlogYorumu> _blogYorumları;
        public string Başlık { get; set; }
        public string Gövde { get; set; }
        public string GövdeGörünüm { get; set; }
        public bool Yorumİzinli { get; set; }
        public string Taglar { get; set; }
        public DateTime? BaşlangıçTarihi { get; set; }
        public DateTime? BitişTarihi { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public virtual bool SitelerdeSınırlı { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        
        public virtual ICollection<BlogYorumu> BlogYorumları
        {
            get { return _blogYorumları ?? (_blogYorumları = new List<BlogYorumu>()); }
            protected set { _blogYorumları = value; }
        }
        
    }
}
