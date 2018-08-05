using Core.Domain.Logging;

namespace Data.Mapping.Logging
{
    public partial class LogMap:TSVarlıkTipiYapılandırması<Log>
    {
        public LogMap()
        {
            this.ToTable("Log");
            this.HasKey(l => l.Id);
            this.Property(l => l.KısaMesaj).IsRequired();
            this.Property(l => l.IpAdresi).HasMaxLength(200);

            this.Ignore(l => l.LogSeviyesi);

            this.HasOptional(l => l.Kullanıcı)
                .WithMany()
                .HasForeignKey(l => l.KullamıcıId)
            .WillCascadeOnDelete(true);
        }
    }
}
