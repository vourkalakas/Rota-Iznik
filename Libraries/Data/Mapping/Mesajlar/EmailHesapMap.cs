using Core.Domain.Mesajlar;

namespace Data.Mapping.Mesajlar
{
    public partial class EmailHesapMap : TSVarlıkTipiYapılandırması<EmailHesabı>
    {
        public EmailHesapMap()
        {
            this.ToTable("EmailHesabı");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Email).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.GörüntülenenAd).HasMaxLength(255);
            this.Property(ea => ea.Host).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.KullanıcıAdı).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.Şifre).IsRequired().HasMaxLength(255);

            this.Ignore(ea => ea.KısaAdı);
        }
    }
}
