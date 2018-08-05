using Core.Domain.Yapılandırma;

namespace Data.Mapping.Yapılandırma
{
    public partial class AyarlarMap : TSVarlıkTipiYapılandırması<Ayarlar>
    {
        public AyarlarMap()
        {
            this.ToTable("Ayarlar");
            this.HasKey(s => s.Id);
            this.Property(s => s.Ad).IsRequired().HasMaxLength(200);
            this.Property(s => s.Değer).IsRequired().HasMaxLength(2000);
        }
    }
}
