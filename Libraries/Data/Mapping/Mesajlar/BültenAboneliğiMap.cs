using Core.Domain.Mesajlar;

namespace Data.Mapping.Mesajlar
{
    public partial class BültenAboneliğiMap : TSVarlıkTipiYapılandırması<BültenAboneliği>
    {
        public BültenAboneliğiMap()
        {
            this.ToTable("BültenAboneliği");
            this.HasKey(nls => nls.Id);
            this.Property(nls => nls.Email).IsRequired().HasMaxLength(255);
        }
    }
}
