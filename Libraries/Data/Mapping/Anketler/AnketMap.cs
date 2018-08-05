using Core.Domain.Anket;
namespace Data.Mapping.Anketler
{
    public partial class AnketMap : TSVarlıkTipiYapılandırması<Anket>
    {
        public AnketMap()
        {
            this.ToTable("Anket");
            this.HasKey(p => p.Id);
            this.Property(p => p.Adı).IsRequired();
        }
    }
}
