using Core.Domain.Kongre;

namespace Data.Mapping.Kongre
{
    public class TransferMap : TSVarlıkTipiYapılandırması<Transfer>
    {
        public TransferMap()
        {
            this.ToTable("Transfer");
            this.HasKey(t => t.Id);
        }
    }

}
