using Core.Domain.Klasör;

namespace Data.Mapping.Klasör
{
    public partial class ÜlkeMap : TSVarlıkTipiYapılandırması<Ülke>
    {
        public ÜlkeMap()
        {
            this.ToTable("Ülke");
            this.HasKey(c => c.Id);
            this.Property(c => c.Adı).IsRequired().HasMaxLength(100);
            this.Property(c => c.İkiHarfIsoKodu).HasMaxLength(2);
            this.Property(c => c.ÜçHarfIsoKodu).HasMaxLength(3);
        }
    }
}
