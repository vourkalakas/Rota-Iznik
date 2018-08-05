using System;
using System.Collections.Generic;
using System.Linq;
using Core.Önbellek;
using Core.Data;
using Core.Domain.Siteler;
using Services.Olaylar;


namespace Services.Siteler
{
    public partial class SiteServisi : ISiteServisi
    {
        #region Constants

        private const string SITELER_ALL_KEY = "TS.siteler.all";
        private const string SITELER_BY_ID_KEY = "TS.siteler.id-{0}";
        private const string SITELER_PATTERN_KEY = "TS.siteler.";

        #endregion

        #region Fields

        private readonly IDepo<Site> _siteDeposu;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;

        #endregion

        #region Ctor

        public SiteServisi(IÖnbellekYönetici önbellekYönetici,
            IDepo<Site> siteDeposu,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._önbellekYönetici = önbellekYönetici;
            this._siteDeposu = siteDeposu;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }

        #endregion

        #region Methods

        public virtual void SiteSil(Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            var tümSiteler = TümSiteler();
            if (tümSiteler.Count == 1)
                throw new Exception("Yapılandırılmış tek mağazayı silemezsiniz");

            _siteDeposu.Sil(site);

            _önbellekYönetici.KalıpİleSil(SITELER_PATTERN_KEY);

            //olay bildirimi
            _olayYayınlayıcı.OlaySilindi(site);
        }
        public virtual IList<Site> TümSiteler()
        {
            string key = SITELER_ALL_KEY;
            return _önbellekYönetici.Al(key, () =>
            {

                var sorgu = from s in _siteDeposu.Tablo
                            orderby s.GörüntülemeSırası, s.Id
                            select s;
                var siteler = sorgu.ToList();
                return siteler;
            });
        }

        public virtual Site SiteAlId(int siteId)
        {
            if (siteId == 0)
                return null;

            string key = string.Format(SITELER_BY_ID_KEY, siteId);
            return _önbellekYönetici.Al(key, () => _siteDeposu.AlId(siteId));
        }

        public virtual void SiteEkle(Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            _siteDeposu.Ekle(site);

            _önbellekYönetici.KalıpİleSil(SITELER_PATTERN_KEY);

            //olay bildirimi
            _olayYayınlayıcı.OlayEklendi(site);
        }

        public virtual void SiteGüncelle(Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            _siteDeposu.Güncelle(site);

            _önbellekYönetici.KalıpİleSil(SITELER_PATTERN_KEY);

            //olay bildirimi
            _olayYayınlayıcı.OlayGüncellendi(site);
        }

        #endregion
    }
}
