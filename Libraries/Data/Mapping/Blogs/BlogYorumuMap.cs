using Core.Domain.Blogs;
namespace Data.Mapping.Blogs
{
    public partial class BlogYorumuMap : TSVarlıkTipiYapılandırması<BlogYorumu>
    {
        public BlogYorumuMap()
        {
            this.ToTable("BlogYorumu");
            this.HasKey(comment => comment.Id);

            this.HasRequired(comment => comment.BlogPost)
                .WithMany(blog => blog.BlogYorumları)
                .HasForeignKey(comment => comment.BlogPostId);

            this.HasRequired(comment => comment.Kullanıcı)
                .WithMany()
                .HasForeignKey(comment => comment.KullanıcıId);

            this.HasRequired(comment => comment.Site)
                .WithMany()
                .HasForeignKey(comment => comment.SiteId);
        }
    }
}
