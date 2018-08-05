using Core.Domain.Medya;

namespace Data.Mapping.Medya
{
    public partial class ResimMap : TSVarlıkTipiYapılandırması<Resim>
    {
        public ResimMap()
        {
            this.ToTable("Resim");
            this.HasKey(p => p.Id);
            this.Property(p => p.ResimBinary).IsMaxLength();
            this.Property(p => p.MimeTipi).IsRequired().HasMaxLength(40);
            this.Property(p => p.SeoDosyaAdı).HasMaxLength(300);
        }
    }
}
