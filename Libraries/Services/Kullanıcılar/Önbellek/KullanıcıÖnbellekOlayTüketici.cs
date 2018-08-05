using Core.Önbellek;
using Core.Domain.Kullanıcılar;
using Core.Altyapı;
using Services.Olaylar;

namespace Services.Kullanıcılar.Önbellek
{
    public partial class KullanıcıÖnbellekOlayTüketici : IMüşteri<KullanıcıŞifreDeğiştirdiOlayı>
    {
        #region Constants

        public const string KULLANICI_ŞİFRE_ÖMRÜ = "ts.kullanıcı.şifreömrü-{0}";

        #endregion

        #region Fields

        private readonly IStatikÖnbellekYönetici _önbellekYönetici;

        #endregion

        #region Ctor

        public KullanıcıÖnbellekOlayTüketici(IStatikÖnbellekYönetici önbellekYönetici)
        {
            this._önbellekYönetici = önbellekYönetici;
        }

        #endregion

        #region Methods

        //şifre değişti
        public void Olay(KullanıcıŞifreDeğiştirdiOlayı olayMesajı)
        {
            _önbellekYönetici.Sil(string.Format(KULLANICI_ŞİFRE_ÖMRÜ, olayMesajı.Şifre.KullanıcıId));
        }

        #endregion
    }
}
