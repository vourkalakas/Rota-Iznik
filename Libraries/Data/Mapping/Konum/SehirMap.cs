using Core.Domain.Konum;

namespace Data.Mapping.EkTanımlamalar
{
    public class SehirMap : TSVarlıkTipiYapılandırması<Sehir>
    {
        public SehirMap()
        {
            this.ToTable("Sehir");
            this.HasKey(t => t.Id);
        }
    }
}
