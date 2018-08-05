using Core.Domain.Görüşmeler;

namespace Data.Mapping.Görüşmeler
{
    public class GorusmeRaporlariMap : TSVarlıkTipiYapılandırması<GorusmeRaporlari>
    {
        public GorusmeRaporlariMap()
        {
            this.ToTable("GorusmeRaporlari");
            this.HasKey(t => t.Id);
        }
    }
}
