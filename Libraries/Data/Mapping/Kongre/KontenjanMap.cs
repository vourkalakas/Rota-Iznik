using Core.Domain.Kongre;

namespace Data.Mapping.Kongre
{
    public class KontenjanMap : TSVarlıkTipiYapılandırması<Kontenjan>
    {
        public KontenjanMap()
        {
            this.ToTable("Kontenjan");
            this.HasKey(t => t.Id);
        }
    }
}
