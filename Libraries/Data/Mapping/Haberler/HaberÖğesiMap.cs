using Core.Domain.Haber;

namespace Data.Mapping.Haberler
{
    public partial class HaberÖğesiMap : TSVarlıkTipiYapılandırması<HaberÖğesi>
    {
        public HaberÖğesiMap()
        {
            this.ToTable("Haberler");
            this.HasKey(ni => ni.Id);
            this.Property(ni => ni.Başlık).IsRequired();
            this.Property(ni => ni.Kısa).IsRequired();
            this.Property(ni => ni.Tam).IsRequired();
            this.Property(ni => ni.MetaKeywords).HasMaxLength(400);
            this.Property(ni => ni.MetaTitle).HasMaxLength(400);
        }
    }
}
