using Core.Domain.Tanımlamalar;

namespace Data.Mapping.Tanımlamalar
{
    public class MusteriMap : TSVarlıkTipiYapılandırması<Musteri>
    {
        public MusteriMap()
        {
            this.ToTable("Musteri");
            this.HasKey(t => t.Id);
        }
    }
}
