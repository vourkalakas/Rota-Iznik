using Core.Domain.Genel;

namespace Data.Mapping.Genel
{
    public partial class GenelÖznitelikMap : TSVarlıkTipiYapılandırması<GenelÖznitelik>
    {
        public GenelÖznitelikMap()
        {
            this.ToTable("GenelÖznitelik");
            this.HasKey(ga => ga.Id);

            this.Property(ga => ga.KeyGroup).IsRequired().HasMaxLength(400);
            this.Property(ga => ga.Key).IsRequired().HasMaxLength(400);
            this.Property(ga => ga.Value).IsRequired();
        }
    }
}
