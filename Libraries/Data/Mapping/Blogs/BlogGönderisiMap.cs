using Core.Domain.Blogs;

namespace Data.Mapping.Blogs
{
    public partial class BlogGönderisiMap : TSVarlıkTipiYapılandırması<BlogPost>
    {
        public BlogGönderisiMap()
        {
            this.ToTable("BlogPost");
            this.HasKey(bp => bp.Id);
            this.Property(bp => bp.Başlık).IsRequired();
            this.Property(bp => bp.Gövde).IsRequired();
            this.Property(bp => bp.MetaKeywords).HasMaxLength(400);
            this.Property(bp => bp.MetaTitle).HasMaxLength(400);
        }
    }
}
