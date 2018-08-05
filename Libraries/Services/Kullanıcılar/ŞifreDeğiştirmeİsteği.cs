using Core.Domain.Kullanıcılar;

namespace Services.Kullanıcılar
{
    public class ŞifreDeğiştirmeİsteği
    {
        public string Email { get; set; }
        public bool İsteğiDoğrula { get; set; }
        public ŞifreFormatı YeniŞifreFormatı { get; set; }
        public string YeniŞifre { get; set; }
        public string EskiŞifre { get; set; }
        public ŞifreDeğiştirmeİsteği(string email, bool isteğiDoğrula,
            ŞifreFormatı yeniŞifreFormatı, string yeniŞifre, string eskiŞifre = "")
        {
            this.Email = email;
            this.İsteğiDoğrula = isteğiDoğrula;
            this.YeniŞifreFormatı = yeniŞifreFormatı;
            this.YeniŞifre = yeniŞifre;
            this.EskiŞifre = eskiŞifre;
        }
    }
}