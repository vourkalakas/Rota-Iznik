using Core.Domain.EkTanımlamalar;

namespace Data.Mapping.EkTanımlamalar
{
    public class UnvanlarMap : TSVarlıkTipiYapılandırması<Unvanlar>
    {
        public UnvanlarMap()
        {
            this.ToTable("Unvanlar");
            this.HasKey(t => t.Id);
        }
    }
}
