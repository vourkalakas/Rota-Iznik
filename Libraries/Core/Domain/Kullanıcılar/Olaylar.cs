namespace Core.Domain.Kullanıcılar
{
    public class KullanıcıBağlandıOlayı
    {
        public KullanıcıBağlandıOlayı(Kullanıcı kullanıcı)
        {
            this.Kullanıcı = kullanıcı;
        }
        public Kullanıcı Kullanıcı
        {
            get; private set;
        }
    }
    public class KullanıcıÇıkışYaptıOlayı
    {
        public KullanıcıÇıkışYaptıOlayı(Kullanıcı kullanıcı)
        {
            this.Kullanıcı = kullanıcı;
        }
        public Kullanıcı Kullanıcı { get; private set; }
    }
    public class KullanıcıKaydolduOlayı
    {
        public KullanıcıKaydolduOlayı(Kullanıcı kullanıcı)
        {
            this.Kullanıcı = kullanıcı;
        }
        public Kullanıcı Kullanıcı
        {
            get; private set;
        }
    }
    public class KullanıcıŞifreDeğiştirdiOlayı
    {
        public KullanıcıŞifreDeğiştirdiOlayı(KullanıcıŞifre şifre)
        {
            this.Şifre = şifre;
        }
        public KullanıcıŞifre Şifre { get; private set; }
    }
}
