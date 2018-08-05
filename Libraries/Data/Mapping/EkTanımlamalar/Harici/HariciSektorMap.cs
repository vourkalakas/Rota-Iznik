using Core.Domain.EkTanımlamalar;

namespace Data.Mapping.EkTanımlamalar
{
    public class HariciSektorMap : TSVarlıkTipiYapılandırması<HariciSektor>
    {
        public HariciSektorMap()
        {
            this.ToTable("HariciSektor");
            this.HasKey(t => t.Id);
        }
    }
}
