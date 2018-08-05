using System;
using System.Collections.ObjectModel;
using Core.Domain.Kullanıcılar;
using Services.Genel;
using System.Diagnostics;
using Core;
using Services.Yapılandırma;

namespace Services.Yardımcılar
{
    public partial class TarihYardımcısı : ITarihYardımcısı
    {
        private readonly IWorkContext _workContext;
        private readonly IGenelÖznitelikServisi _genelÖznitelikServisi;
        private readonly TarihAyarları _tarihAyarları;
        private readonly IAyarlarServisi _ayarlarServisi;
        public TarihYardımcısı(IWorkContext workContext,
            IGenelÖznitelikServisi genelÖznitelikServisi,
            TarihAyarları tarihAyarları,
            IAyarlarServisi ayarlarServisi
            )
        {
            this._workContext = workContext;
            this._genelÖznitelikServisi = genelÖznitelikServisi;
            this._tarihAyarları = tarihAyarları;
            this._ayarlarServisi = ayarlarServisi;
        }
        public TimeZoneInfo SiteVarsayılanZamanDilimi
        {
            get
            {
                TimeZoneInfo zamanDilimiBilgisi = null;
                try
                {
                    if (!String.IsNullOrEmpty(_tarihAyarları.SiteVarsayılanZamanDilimiId))
                        zamanDilimiBilgisi = ZamanDilimiBulId(_tarihAyarları.SiteVarsayılanZamanDilimiId);
                }
                catch (Exception exc)
                {
                    Debug.Write(exc.ToString());
                }

                if (zamanDilimiBilgisi == null)
                    zamanDilimiBilgisi = TimeZoneInfo.Local;

                return zamanDilimiBilgisi;
            }
            set
            {
                string varsayılanZamanDilimiId = string.Empty;
                if (value != null)
                {
                    varsayılanZamanDilimiId = value.Id;
                }

                _tarihAyarları.SiteVarsayılanZamanDilimiId = varsayılanZamanDilimiId;
                _ayarlarServisi.AyarKaydet(_tarihAyarları);
            }
        }
        public TimeZoneInfo MevcutZamanDilimi
        {
            get
            {
                return KullanıcıZamanDiliminiAl(_workContext.MevcutKullanıcı);
            }
            set
            {
                if (!_tarihAyarları.KullanıcıZamanDilimiAyarlamasıİzinli)
                    return;

                string zamanDilimiId = string.Empty;
                if (value != null)
                {
                    zamanDilimiId = value.Id;
                }

                _genelÖznitelikServisi.ÖznitelikKaydet(_workContext.MevcutKullanıcı,
                    SistemKullanıcıÖznitelikAdları.ZamanDilimiId, zamanDilimiId);
            }
        }

        public TimeZoneInfo KullanıcıZamanDiliminiAl(Kullanıcı kullanıcı)
        {
            TimeZoneInfo zamanDilimiBilgisi = null;
            if (_tarihAyarları.KullanıcıZamanDilimiAyarlamasıİzinli)
            {
                string zamanDilimiId = string.Empty;
                if (kullanıcı != null)
                    zamanDilimiId = kullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.ZamanDilimiId, _genelÖznitelikServisi);

                try
                {
                    if (!String.IsNullOrEmpty(zamanDilimiId))
                        zamanDilimiBilgisi = ZamanDilimiBulId(zamanDilimiId);
                }
                catch (Exception exc)
                {
                    Debug.Write(exc.ToString());
                }
            }

            if (zamanDilimiBilgisi == null)
                zamanDilimiBilgisi = this.SiteVarsayılanZamanDilimi;

            return zamanDilimiBilgisi;
        }

        public DateTime KullanıcıZamanınaDönüştür(DateTime dt)
        {
            return KullanıcıZamanınaDönüştür(dt, dt.Kind);
        }

        public DateTime KullanıcıZamanınaDönüştür(DateTime dt, DateTimeKind kaynakTarihTürü)
        {
            dt = DateTime.SpecifyKind(dt, kaynakTarihTürü);
            var kullanıcıMevcutZamanDilimiBilgisi = this.MevcutZamanDilimi;
            return TimeZoneInfo.ConvertTime(dt, kullanıcıMevcutZamanDilimiBilgisi);
        }

        public DateTime KullanıcıZamanınaDönüştür(DateTime dt, TimeZoneInfo kaynakZamanDilimi)
        {
            var kullanıcıMevcutZamanDilimiBilgisi = this.MevcutZamanDilimi;
            return KullanıcıZamanınaDönüştür(dt, kaynakZamanDilimi, kullanıcıMevcutZamanDilimiBilgisi);
        }

        public DateTime KullanıcıZamanınaDönüştür(DateTime dt, TimeZoneInfo kaynakZamanDilimi, TimeZoneInfo hedefZamanDilimi)
        {
            return TimeZoneInfo.ConvertTime(dt, kaynakZamanDilimi, hedefZamanDilimi);
        }

        public ReadOnlyCollection<TimeZoneInfo> SistemZamanDilimiAl()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }

        public DateTime UtcyeDönüştür(DateTime dt)
        {
            return UtcyeDönüştür(dt, dt.Kind);
        }

        public DateTime UtcyeDönüştür(DateTime dt, DateTimeKind kaynakTarihTürü)
        {
            dt = DateTime.SpecifyKind(dt, kaynakTarihTürü);
            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }

        public DateTime UtcyeDönüştür(DateTime dt, TimeZoneInfo kaynakZamanDilimi)
        {
            if (kaynakZamanDilimi.IsInvalidTime(dt))
            {
                return dt;
            }
            return TimeZoneInfo.ConvertTimeToUtc(dt, kaynakZamanDilimi);
        }

        public virtual TimeZoneInfo ZamanDilimiBulId(string id)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
    }
}
