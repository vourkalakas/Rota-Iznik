using Core.Domain.Kongre;

namespace Data.Mapping.Kongre
{
    public class KonaklamaMap : TSVarlıkTipiYapılandırması<Konaklama>
    {
        public KonaklamaMap()
        {
            this.ToTable("Konaklama");
            this.HasKey(t => t.Id);
        }
    }

}
