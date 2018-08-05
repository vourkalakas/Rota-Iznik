
using Core.Domain.Kullanıcılar;

namespace Services.Kullanıcılar
{
    public class KullanıcıKayıtİsteği
    {
        public KullanıcıKayıtİsteği(Kullanıcı kullanıcı, string email, string kullanıcıAdı,
            string şifre,
            ŞifreFormatı şifreFormatı,
            int siteId,
            bool onaylandı = true)
        {
            this.Kullanıcı = kullanıcı;
            this.Email = email;
            this.KullanıcıAdı = kullanıcıAdı;
            this.Şifre = şifre;
            this.ŞifreFormatı = şifreFormatı;
            this.SiteId = siteId;
            this.Onaylandı = onaylandı;
        }
        public Kullanıcı Kullanıcı { get; set; }
        public string Email { get; set; }
        public string KullanıcıAdı { get; set; }
        public string Şifre { get; set; }
        public ŞifreFormatı ŞifreFormatı { get; set; }
        public int SiteId { get; set; }
        public bool Onaylandı { get; set; }
    }
}
