using Core.Domain.Kongre;

namespace Data.Mapping.Kongre
{
    public class KursMap : TSVarlıkTipiYapılandırması<Kurs>
    {
        public KursMap()
        {
            this.ToTable("Kurs");
            this.HasKey(t => t.Id);
        }
    }

}
