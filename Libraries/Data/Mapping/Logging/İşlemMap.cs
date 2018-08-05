using Core.Domain.Logging;

namespace Data.Mapping.Logging
{
    public partial class İşlemMap : TSVarlıkTipiYapılandırması<İşlem>
    {
        public İşlemMap()
        {
            this.ToTable("İşlem");
            this.HasKey(al => al.Id);
            this.Property(al => al.Yorum).IsRequired();
            this.Property(al => al.IpAdresi).HasMaxLength(200);

            this.HasRequired(al => al.İşlemTipi)
                .WithMany()
                .HasForeignKey(al => al.İşlemTipiId);

            this.HasRequired(al => al.Kullanıcı)
                .WithMany()
                .HasForeignKey(al => al.KullanıcıId);
        }
    }
}
