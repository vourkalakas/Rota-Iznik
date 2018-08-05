namespace Core.Domain.Siparişler
{
    public enum İadeİsteğiDurumu
    {
        Askıda = 0,
        Alındı = 10,
        İadeYetkili = 20,
        Onarıldı = 30,
        GeriÖdendi = 40,
        İstekReddedildi = 50,
        İptalEdildi = 60,
    }
}