using Core.Domain.Sayfalar;

namespace Data.Mapping.Sayfalar
{
    public class SayfaMap : TSVarlıkTipiYapılandırması<Sayfa>
    {
        public SayfaMap()
        {
            this.ToTable("Sayfa");
            this.HasKey(t => t.Id);
        }
    }
}
