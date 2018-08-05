using Core.Domain.Mesajlar;

namespace Data.Mapping.Mesajlar
{
    public class MesajlarMap : TSVarlıkTipiYapılandırması<Mesaj>
    {
        public MesajlarMap()
        {
            this.ToTable("Mesajlar");
            this.HasKey(t => t.Id);
        }
    }
}
