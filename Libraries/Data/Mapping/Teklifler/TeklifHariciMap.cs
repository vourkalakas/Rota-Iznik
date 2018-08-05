using Core.Domain.Teklif;

namespace Data.Mapping.Teklifler
{
    public class TeklifHariciMap : TSVarlıkTipiYapılandırması<TeklifHarici>
    {
        public TeklifHariciMap()
        {
            this.ToTable("TeklifHarici");
            this.HasKey(t => t.Id);
        }
    }
}
