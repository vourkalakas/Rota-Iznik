using Core.Domain.Kullanıcılar;

namespace Services.KimlikDoğrulama.Harici
{
    public class KullanıcıHariciOlarakOtomatikKaydedildi
    {
        public KullanıcıHariciOlarakOtomatikKaydedildi(Kullanıcı kullanıcı, HariciYetkilendirmeParametreleri parametreler)
        {
            this.Kullanıcı = kullanıcı;
            this.YetkilendirmeParametreleri = parametreler;
        }
        public Kullanıcı Kullanıcı { get; }
        public HariciYetkilendirmeParametreleri YetkilendirmeParametreleri { get; }
    }
}
