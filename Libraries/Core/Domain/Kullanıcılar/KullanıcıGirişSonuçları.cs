namespace Core.Domain.Kullanıcılar
{
   
    public enum KullanıcıGirişSonuçları
    {
        Başarılı = 1,
        KullanıcıMevcutDeğil = 2,
        HatalıŞifre = 3,
        AktifDeğil = 4,
        Silindi = 5,
        KayıtlıDeğil = 6,
        Kilitlendi = 7,
    }
}
