using Core.Domain.Seo;

namespace Data.Mapping.Seo
{
    public partial class UrlKaydıMap : TSVarlıkTipiYapılandırması<UrlKaydı>
    {
        public UrlKaydıMap()
        {
            this.ToTable("UrlKaydı");
            this.HasKey(lp => lp.Id);

            this.Property(lp => lp.VarlıkAdı).IsRequired().HasMaxLength(400);
            this.Property(lp => lp.Slug).IsRequired().HasMaxLength(400);
        }
    }
}
