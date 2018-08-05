using Core.Domain.Güvenlik;
namespace Data.Mapping.Güvenlik
{
    public partial class AclKaydıMap : TSVarlıkTipiYapılandırması<AclKaydı>
    {
        public AclKaydıMap()
        {
            this.ToTable("AclKaydı");
            this.HasKey(ar => ar.Id);

            this.Property(ar => ar.VarlıkAdı).IsRequired().HasMaxLength(400);

            this.HasRequired(ar => ar.KullanıcıRolü)
                .WithMany()
                .HasForeignKey(ar => ar.KullanıcıRolId)
                .WillCascadeOnDelete(true);
        }
    }
}
