using Core.Domain.EkTanımlamalar;

namespace Data.Mapping.EkTanımlamalar
{
    public class TeklifKalemiMap : TSVarlıkTipiYapılandırması<TeklifKalemi>
    {
        public TeklifKalemiMap()
        {
            this.ToTable("TeklifOgesi");
            this.HasKey(t => t.Id);
            this.Property(t => t.Adı).IsRequired().HasMaxLength(100);
        }
    }
}
