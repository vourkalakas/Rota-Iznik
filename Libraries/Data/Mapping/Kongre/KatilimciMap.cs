using Core.Domain.Kongre;
namespace Data.Mapping.Kongre
{
    public class KatilimciMap : TSVarlıkTipiYapılandırması<Katilimci>
    {
        public KatilimciMap()
        {
            this.ToTable("Katilimci");
            this.HasKey(t => t.Id);
        }
    }

}
