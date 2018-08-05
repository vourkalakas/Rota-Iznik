using Core.Domain.Tanımlamalar;

namespace Data.Mapping.Tanımlamalar
{
    public class YDAcenteMap : TSVarlıkTipiYapılandırması<YDAcente>
    {
        public YDAcenteMap()
        {
            this.ToTable("YDAcente");
            this.HasKey(t => t.Id);
        }
    }
}
