using Core.Domain.Notlar;

namespace Data.Mapping.Mesajlar
{
    public partial class NotMap : TSVarlıkTipiYapılandırması<Not>
    {
        public NotMap()
        {
            this.ToTable("Notlar");
            this.HasKey(ea => ea.Id);
        }
    }
}
