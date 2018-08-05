using Core.Domain.Sayfalar;
namespace Data.Mapping.Sayfalar
{
    public partial class SayfaTemaMap : TSVarlıkTipiYapılandırması<SayfaTema>
    {
        public SayfaTemaMap()
        {
            this.ToTable("SayfaTema");
            this.HasKey(t => t.Id);
            this.Property(t => t.Adı).IsRequired().HasMaxLength(400);
            this.Property(t => t.Yolu).IsRequired().HasMaxLength(400);
        }
    }
}
