using Core.Domain.Siteler;

namespace Data.Mapping.Siteler
{
    public partial class SiteMappingMap : TSVarlıkTipiYapılandırması<SiteMapping>
    {
        public SiteMappingMap()
        {
            this.ToTable("SiteMapping");
            this.HasKey(sm => sm.Id);

            this.Property(sm => sm.VarlıkAdı).IsRequired().HasMaxLength(400);

            this.HasRequired(sm => sm.Site)
                .WithMany()
                .HasForeignKey(sm => sm.SiteId)
                .WillCascadeOnDelete(true);
        }
    }
}
