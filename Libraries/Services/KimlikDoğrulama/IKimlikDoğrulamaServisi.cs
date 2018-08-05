using Core.Domain.Kullanıcılar;

namespace Services.KimlikDoğrulama
{
    public partial interface IKimlikDoğrulamaServisi
    {
        void Giriş(Kullanıcı kullanıcı, bool kalıcıÇerezOluştur);
        void Çıkış();
        Kullanıcı KimliğiDoğrulananKullanıcı();
    }
}
