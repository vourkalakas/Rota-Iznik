using Core.Domain.Kullanıcılar;

namespace Data.Mapping.Kullanıcılar
{
    public partial class KullanıcıŞifreMap : TSVarlıkTipiYapılandırması<KullanıcıŞifre>
    {
        public KullanıcıŞifreMap()
        {
            this.ToTable("KullanıcıŞifre");
            this.HasKey(şifre => şifre.Id);

            this.HasRequired(şifre => şifre.Kullanıcı)
                .WithMany()
                .HasForeignKey(şifre => şifre.KullanıcıId);

            this.Ignore(şifre => şifre.ŞifreFormatı);
        }
    }
}
