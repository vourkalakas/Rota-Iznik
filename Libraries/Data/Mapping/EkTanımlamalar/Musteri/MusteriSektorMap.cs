using Core.Domain.EkTanımlamalar;

namespace Data.Mapping.EkTanımlamalar
{
    public class MusteriSektorMap : TSVarlıkTipiYapılandırması<MusteriSektor>
    {
        public MusteriSektorMap()
        {
            this.ToTable("MusteriSektor");
            this.HasKey(t => t.Id);
        }
    }
}
