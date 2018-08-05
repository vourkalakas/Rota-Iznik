using Core.Domain.EkTanımlamalar;

namespace Data.Mapping.EkTanımlamalar
{
    public class BankaMap : TSVarlıkTipiYapılandırması<Banka>
    {
        public BankaMap()
        {
            this.ToTable("Banka");
            this.HasKey(t => t.Id);
        }
    }
}
