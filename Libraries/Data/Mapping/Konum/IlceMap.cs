using Core.Domain.Konum;

namespace Data.Mapping.EkTanımlamalar
{
    public class IlceMap : TSVarlıkTipiYapılandırması<Ilce>
    {
        public IlceMap()
        {
            this.ToTable("Ilce");
            this.HasKey(t => t.Id);
        }
    }
}
