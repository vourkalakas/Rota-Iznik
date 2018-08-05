using Core.Domain.Hint;

namespace Data.Mapping.Katalog
{
    public class HintMap : TSVarlıkTipiYapılandırması<Hints>
    {
        public HintMap()
        {
            this.ToTable("Hint");
            this.HasKey(t => t.Id);
        }
    }
}
