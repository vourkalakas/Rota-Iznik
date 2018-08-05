using Core.Domain.Genel;

namespace Services.Genel
{
    public partial interface IAdresServisi
    {
        void AdresSil(Adres adres);
        void AdresAlÜlkeId(int ülkeId);
        Adres AdresAl(int adresId);
        void AdresEkle(Adres adres);
        void AdresGüncelle(Adres adres);
        bool AdresGeçerli(Adres adres);
    }
}
