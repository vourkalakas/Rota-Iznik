using Core.Domain.Kullanıcılar;

namespace Data.Mapping.Kullanıcılar
{
    public partial class KullanıcıRolüMap : TSVarlıkTipiYapılandırması<KullanıcıRolü>
    {
        public KullanıcıRolüMap()
        {
            this.ToTable("KullanıcıRolü");
            this.HasKey(cr => cr.Id);
            this.Property(cr => cr.Adı).IsRequired().HasMaxLength(255);
            this.Property(cr => cr.SistemAdı).HasMaxLength(255);
        }
    }
}
