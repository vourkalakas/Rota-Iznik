using Core.Domain.Kullanıcılar;

namespace Services.Kullanıcılar
{
    
    public partial interface IKullanıcıKayıtServisi
    {
        KullanıcıGirişSonuçları KullanıcıDoğrula(string kullanıcıAdıVeyaEmail, string şifre);
        KullanıcıKayıtSonuçları KullanıcıKaydet(KullanıcıKayıtİsteği istek);
        ŞifreDeğiştirmeSonuçları ŞifreDeğiştir(ŞifreDeğiştirmeİsteği istek);
        void EmailAyarla(Kullanıcı kullanıcı, string yeniEmail, bool doğrulamaGerekli);
        void KullanıcıAdıAyarla(Kullanıcı kullanıcı, string yeniKullanıcıAdı);
    }
}
