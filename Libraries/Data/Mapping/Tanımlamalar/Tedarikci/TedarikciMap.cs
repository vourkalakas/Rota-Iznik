using Core.Domain.Tanımlamalar;

namespace Data.Mapping.Tanımlamalar
{
    public class TedarikciMap : TSVarlıkTipiYapılandırması<Tedarikci>
    {
        public TedarikciMap()
        {
            this.ToTable("Tedarikci");
            this.HasKey(t => t.Id);
        }
    }
}
