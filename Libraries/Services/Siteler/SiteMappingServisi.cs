using System;
using System.Collections.Generic;
using Core;
using Core.Domain.Siteler;
using Core.Data;
using Core.Önbellek;
using Services.Olaylar;
using Core.Domain.Katalog;
using System.Linq;

namespace Services.Siteler
{
    public partial class SiteMappingServisi : ISiteMappingServisi
    {
        private const string SITEMAPPING_BY_ENTITYID_NAME_KEY = "sitemapping.entityid-name-{0}-{1}";
        private const string SITEMAPPING_PATTERN_KEY = "sitemapping.";
        private readonly IDepo<SiteMapping> _siteMappingDepo;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly ISiteContext _siteContext;
        private readonly KatalogAyarları _katalogAyarları;
        public SiteMappingServisi(IDepo<SiteMapping> siteMappingDepo,
            IÖnbellekYönetici önbellekYönetici,
            IOlayYayınlayıcı olayYayınlayıcı,
            ISiteContext siteContext,
            KatalogAyarları katalogAyarları)
        {
            this._siteMappingDepo = siteMappingDepo;
            this._önbellekYönetici = önbellekYönetici;
            this._siteContext = siteContext;
            this._katalogAyarları = katalogAyarları;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }
        public virtual int[] ErişimİleSiteMappingleriAl<T>(T varlık) where T : TemelVarlık, ISiteMappingDestekli
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            int varlıkId = varlık.Id;
            string varlıkAdı = typeof(T).Name;

            string key = string.Format(SITEMAPPING_BY_ENTITYID_NAME_KEY, varlıkId, varlıkAdı);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = from sm in _siteMappingDepo.Tablo
                            where sm.VarlıkId == varlıkId &&
                            sm.VarlıkAdı == varlıkAdı
                            select sm.SiteId;
                return sorgu.ToArray();
            });
        }

        public virtual SiteMapping SiteMappingAlId(int siteMappingId)
        {
            if (siteMappingId == 0)
                return null;
            return _siteMappingDepo.AlId(siteMappingId);
        }

        public virtual void SiteMappingEkle(SiteMapping siteMapping)
        {
            if (siteMapping == null)
                throw new ArgumentNullException("siteMapping");
            _siteMappingDepo.Ekle(siteMapping);
            _önbellekYönetici.KalıpİleSil(SITEMAPPING_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(siteMapping);
        }

        public virtual void SiteMappingEkle<T>(T entity, int siteId) where T : TemelVarlık, ISiteMappingDestekli
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (siteId == 0)
                throw new ArgumentOutOfRangeException("storeId");

            int varlıkId = entity.Id;
            string varlıkAdı = typeof(T).Name;

            var siteMapping = new SiteMapping
            {
                VarlıkId = varlıkId,
                VarlıkAdı = varlıkAdı,
                SiteId = siteId
            };
            SiteMappingEkle(siteMapping);
        }

        public virtual void SiteMappingGüncelle(SiteMapping siteMapping)
        {
            if (siteMapping == null)
                throw new ArgumentNullException("siteMapping");
            _siteMappingDepo.Güncelle(siteMapping);
            _önbellekYönetici.KalıpİleSil(SITEMAPPING_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(siteMapping);
        }

        public virtual IList<SiteMapping> SiteMappingleriAl<T>(T varlık) where T : TemelVarlık, ISiteMappingDestekli
        {
            if (varlık == null)
                throw new ArgumentNullException("entity");

            int varlıkId = varlık.Id;
            string varlıkAdı = typeof(T).Name;

            var sorgu = from sm in _siteMappingDepo.Tablo
                        where sm.VarlıkId == varlıkId &&
                        sm.VarlıkAdı == varlıkAdı
                        select sm;
            var siteMappingler = sorgu.ToList();
            return siteMappingler;
        }

        public virtual void SiteMappingSil(SiteMapping siteMapping)
        {
            if (siteMapping == null)
                throw new ArgumentNullException("siteMapping");
            _siteMappingDepo.Sil(siteMapping);
            _önbellekYönetici.KalıpİleSil(SITEMAPPING_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(siteMapping);
        }

        public virtual bool YetkiVer<T>(T varlık) where T : TemelVarlık, ISiteMappingDestekli
        {
            return YetkiVer(varlık, _siteContext.MevcutSite.Id);
        }

        public virtual bool YetkiVer<T>(T varlık, int siteId) where T : TemelVarlık, ISiteMappingDestekli
        {
            if (varlık == null)
                return false;

            if (siteId == 0)
                return true;

            if (_katalogAyarları.IgnoreStoreLimitations)
                return true;

            if (!varlık.SitelerdeSınırlı)
                return true;

            foreach (var erişimliSiteId in ErişimİleSiteMappingleriAl(varlık))
                if (siteId == erişimliSiteId)
                    return true;
            return false;
        }
    }
}
