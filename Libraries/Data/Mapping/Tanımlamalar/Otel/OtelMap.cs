using Core.Domain.Tanımlamalar;
namespace Data.Mapping.Tanımlamalar
{
    public class OtelMap : TSVarlıkTipiYapılandırması<Otel>
    {
        public OtelMap()
        {
            this.ToTable("Otel");
            this.HasKey(t => t.Id);
        }
    }
}
