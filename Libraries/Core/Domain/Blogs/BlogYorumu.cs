using Core.Domain.Kullanıcılar;
using Core.Domain.Siteler;
using System;

namespace Core.Domain.Blogs
{
    public partial class BlogYorumu : TemelVarlık
    {
        public int KullanıcıId { get; set; }
        public string YorumYazısı { get; set; }
        public bool Onaylandı { get; set; }
        public int SiteId { get; set; }
        public int BlogPostId { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public virtual Kullanıcı Kullanıcı { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public virtual Site Site { get; set; }
    }
}
