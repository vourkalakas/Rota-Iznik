using Core.Domain.EkTanımlamalar;

namespace Data.Mapping.EkTanımlamalar
{
    public class TedarikciSektorMap : TSVarlıkTipiYapılandırması<TedarikciSektor>
    {
        public TedarikciSektorMap()
        {
            this.ToTable("TedarikciSektor");
            this.HasKey(t => t.Id);
        }
    }
}
