using Core.Domain.Kullanıcılar;

namespace Data.Mapping.Kullanıcılar
{
    public partial class KullanıcıMap : TSVarlıkTipiYapılandırması<Kullanıcı>
    {
        public KullanıcıMap()
        {
            this.ToTable("Kullanıcı");
            this.HasKey(c => c.Id);
            this.Property(u => u.KullanıcıAdı).HasMaxLength(1000);
            this.Property(u => u.Email).HasMaxLength(1000);
            this.Property(u => u.EmailDoğrulandı).HasMaxLength(1000);
            this.Property(u => u.SistemAdı).HasMaxLength(400);

            this.HasMany(c => c.KullanıcıRolleri)
                .WithMany()
                .Map(m => m.ToTable("Kullanıcı_KullanıcıRol_Mapping"));
            
            this.HasMany(c => c.Adresler)
                .WithMany()
                .Map(m => m.ToTable("KullanıcıAdresi"));
            this.HasOptional(c => c.FaturaAdresi);
            
        }
    }
}
