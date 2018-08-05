using Core.Domain.Siteler;

namespace Data.Mapping.Siteler
{
    public partial class SiteMap : TSVarlıkTipiYapılandırması<Site>
    {
        public SiteMap()
        {
            this.ToTable("Site");
            this.HasKey(s => s.Id);
            this.Property(s => s.Adı).IsRequired().HasMaxLength(400);
            this.Property(s => s.Url).IsRequired().HasMaxLength(400);
            this.Property(s => s.GüvenliUrl).HasMaxLength(400);
            this.Property(s => s.Hosts).HasMaxLength(1000);

            this.Property(s => s.ŞirketAdı).HasMaxLength(1000);
            this.Property(s => s.ŞirketAdresi).HasMaxLength(1000);
            this.Property(s => s.ŞirketTelefon).HasMaxLength(1000);
        }
    }
}
