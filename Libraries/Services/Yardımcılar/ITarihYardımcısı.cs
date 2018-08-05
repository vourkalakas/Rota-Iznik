using Core.Domain.Kullanıcılar;
using System;
using System.Collections.ObjectModel;

namespace Services.Yardımcılar
{
    public partial interface ITarihYardımcısı
    {
        TimeZoneInfo ZamanDilimiBulId(string id);
        ReadOnlyCollection<TimeZoneInfo> SistemZamanDilimiAl();
        DateTime KullanıcıZamanınaDönüştür(DateTime dt);
        DateTime KullanıcıZamanınaDönüştür(DateTime dt, DateTimeKind kaynakTarihTürü);
        DateTime KullanıcıZamanınaDönüştür(DateTime dt, TimeZoneInfo kaynakZamanDilimi);
        DateTime KullanıcıZamanınaDönüştür(DateTime dt, TimeZoneInfo kaynakZamanDilimi, TimeZoneInfo hedefZamanDilimi);
        DateTime UtcyeDönüştür(DateTime dt);
        DateTime UtcyeDönüştür(DateTime dt, DateTimeKind kaynakTarihTürü);
        DateTime UtcyeDönüştür(DateTime dt, TimeZoneInfo kaynakZamanDilimi);
        TimeZoneInfo KullanıcıZamanDiliminiAl(Kullanıcı kullanıcı);
        TimeZoneInfo SiteVarsayılanZamanDilimi { get; set; }
        TimeZoneInfo MevcutZamanDilimi { get; set; }
    }
}
