using Core.Domain.Kongre;
namespace Data.Mapping.Kongre
{
    public class RefakatciMap : TSVarlıkTipiYapılandırması<Refakatci>
    {
        public RefakatciMap()
        {
            this.ToTable("Refakatci");
            this.HasKey(t => t.Id);
        }
    }

}
