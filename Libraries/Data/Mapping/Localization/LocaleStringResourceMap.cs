using Core.Domain.Localization;

namespace Data.Mapping.Localization
{
    public partial class LocaleStringResourceMap : TSVarlıkTipiYapılandırması<LocaleStringResource>
    {
        public LocaleStringResourceMap()
        {
            this.ToTable("LocaleStringResource");
            this.HasKey(lsr => lsr.Id);
            this.Property(lsr => lsr.ResourceName).IsRequired().HasMaxLength(200);
            this.Property(lsr => lsr.ResourceValue).IsRequired();

            this.HasRequired(lsr => lsr.Language)
                .WithMany()
                .HasForeignKey(lsr => lsr.LanguageId);
        }
    }
}
