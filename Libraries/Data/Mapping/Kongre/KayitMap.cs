using Core.Domain.Kongre;

namespace Data.Mapping.Kongre
{
    public class KayitMap : TSVarlıkTipiYapılandırması<Kayit>
    {
        public KayitMap()
        {
            this.ToTable("Kayit");
            this.HasKey(t => t.Id);
        }
    }
}
