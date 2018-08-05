using Core.Domain.Haber;

namespace Data.Mapping.Haberler
{
    public partial class HaberYorumuMap : TSVarlıkTipiYapılandırması<HaberYorumu>
    {
        public HaberYorumuMap()
        {
            this.ToTable("HaberYorumu");
            this.HasKey(comment => comment.Id);

            this.HasRequired(comment => comment.HaberÖğesi)
                .WithMany(news => news.HaberYorumu)
                .HasForeignKey(comment => comment.HaberÖğesiId);

            this.HasRequired(comment => comment.Kullanıcı)
                .WithMany()
                .HasForeignKey(comment => comment.KullanıcıId);

            this.HasRequired(comment => comment.Site)
                .WithMany()
                .HasForeignKey(comment => comment.SiteId);
        }
    }
}
