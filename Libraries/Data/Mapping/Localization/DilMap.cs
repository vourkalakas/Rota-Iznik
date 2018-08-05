using Core.Domain.Localization;

namespace Data.Mapping.Localization
{
    public partial class DilMap : TSVarlıkTipiYapılandırması<Dil>
    {
        public DilMap()
        {
            this.ToTable("Dil");
            this.HasKey(l => l.Id);
            this.Property(l => l.Adı).IsRequired().HasMaxLength(100);
            this.Property(l => l.DilKültürü).IsRequired().HasMaxLength(20);
            this.Property(l => l.ÖzelSeoKodu).HasMaxLength(2);
            this.Property(l => l.BayrakResmiDosyaAdı).HasMaxLength(50);
        }
    }
}
