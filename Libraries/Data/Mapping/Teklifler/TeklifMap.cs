using Core.Domain.Teklif;

namespace Data.Mapping.Teklifler
{
    public class TeklifMap : TSVarlıkTipiYapılandırması<Teklif>
    {
        public TeklifMap()
        {
            this.ToTable("Teklif");
            this.HasKey(t => t.Id);
        }
    }
}
