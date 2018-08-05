using Core.Domain.Teklif;

namespace Data.Mapping.Teklifler
{
    public class BagliTeklifOgesiHariciMap : TSVarlıkTipiYapılandırması<BagliTeklifOgesiHarici>
    {
        public BagliTeklifOgesiHariciMap()
        {
            this.ToTable("BagliTeklifOgesiHarici");
            this.HasKey(t => t.Id);
        }
    }
}
