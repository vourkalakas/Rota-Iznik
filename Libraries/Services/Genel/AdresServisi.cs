using System;
using Core.Domain.Genel;
using Core.Data;
using Core.Önbellek;
using Services.Olaylar;

namespace Services.Genel
{
    public partial class AdresServisi : IAdresServisi
    {
        #region Constants

        private const string ADRESLER_ID_KEY = "adres.id-{0}";
        private const string ADRESLER_PATTERN_KEY = "adres.";

        #endregion

        #region Fields

        private readonly IDepo<Adres> _adresDepo;
        //private readonly IÜlkeServisi _ülkeServisi;
        //private readonly IAdresÖznitelikServisi _AdresÖznitelikServisi;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        //private readonly AdresAyarları _adresAyarları;
        private readonly IÖnbellekYönetici _önbellekYönetici;

        #endregion
        public Adres AdresAl(int adresId)
        {
            throw new NotImplementedException();
        }

        public void AdresAlÜlkeId(int ülkeId)
        {
            throw new NotImplementedException();
        }

        public void AdresEkle(Adres adres)
        {
            throw new NotImplementedException();
        }

        public bool AdresGeçerli(Adres adres)
        {
            throw new NotImplementedException();
        }

        public void AdresGüncelle(Adres adres)
        {
            throw new NotImplementedException();
        }

        public void AdresSil(Adres adres)
        {
            throw new NotImplementedException();
        }
    }
}
