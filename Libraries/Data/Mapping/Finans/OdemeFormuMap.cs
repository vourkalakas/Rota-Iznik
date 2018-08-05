using Core.Domain.Finans;
namespace Data.Mapping.Finans
{
    public class OdemeFormuMap : TSVarlıkTipiYapılandırması<OdemeFormu>
    {
        public OdemeFormuMap()
        {
            this.ToTable("OdemeFormu");
            this.HasKey(t => t.Id);
        }
    }
}
