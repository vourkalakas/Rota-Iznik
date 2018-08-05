using Core.Domain.Katalog;

namespace Data.Mapping.Katalog
{
    public partial class KategoriMap : TSVarlıkTipiYapılandırması<Kategori>
    {
        public KategoriMap()
        {
            this.ToTable("Kategori");
            this.HasKey(c => c.Id);
            this.Property(c => c.Adı).IsRequired().HasMaxLength(400);
            this.Property(c => c.MetaKeywords).HasMaxLength(400);
            this.Property(c => c.MetaTitle).HasMaxLength(400);
            this.Property(c => c.SayfaBüyüklüğüSeçenekleri).HasMaxLength(200);
        }
    }
}
