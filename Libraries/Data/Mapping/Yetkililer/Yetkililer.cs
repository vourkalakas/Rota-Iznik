using Core.Domain.Yetkililer;

namespace Data.Mapping
{
    public class YetkililerMap : TSVarlıkTipiYapılandırması<Yetkili>
    {
        public YetkililerMap()
        {
            this.ToTable("Yetkililer");
            this.HasKey(t => t.Id);
        }
    }
}
