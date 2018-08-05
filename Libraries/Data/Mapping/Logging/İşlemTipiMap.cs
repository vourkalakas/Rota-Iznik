using Core.Domain.Logging;

namespace Data.Mapping.Logging
{
    public partial class İşlemTipiMap : TSVarlıkTipiYapılandırması<İşlemTipi>
    {
        public İşlemTipiMap()
        {
            this.ToTable("İşlemTipi");
            this.HasKey(alt => alt.Id);

            this.Property(alt => alt.SistemAnahtarKelimeleri).IsRequired().HasMaxLength(100);
            this.Property(alt => alt.Adı).IsRequired().HasMaxLength(200);
        }
    }
}
