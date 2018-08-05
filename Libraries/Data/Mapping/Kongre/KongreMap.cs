using Core.Domain.Kongre;

namespace Data.Mapping.Kongre
{
    public class KongreMap : TSVarlıkTipiYapılandırması<Kongreler>
    {
        public KongreMap()
        {
            this.ToTable("Kongre");
            this.HasKey(t => t.Id);
        }
    }

}
