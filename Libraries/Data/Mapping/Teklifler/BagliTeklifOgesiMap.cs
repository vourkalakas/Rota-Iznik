using Core.Domain.Teklif;

namespace Data.Mapping.Teklifler
{
    public class BagliTeklifOgesiMap : TSVarlıkTipiYapılandırması<BagliTeklifOgesi>
    {
        public BagliTeklifOgesiMap()
        {
            this.ToTable("BagliTeklifOgesi");
            this.HasKey(t => t.Id);
        }
    }
}
