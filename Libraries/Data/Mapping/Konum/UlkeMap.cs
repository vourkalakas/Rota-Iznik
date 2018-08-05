using Core.Domain.Konum;

namespace Data.Mapping.EkTanımlamalar
{
    public class UlkeMap : TSVarlıkTipiYapılandırması<Ulke>
    {
        public UlkeMap()
        {
            this.ToTable("Ulkeler");
            this.HasKey(t => t.Id);
        }
    }
}
