using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Data;
using Core.Domain.EkTanımlamalar;
using Core.Önbellek;
using Services.Olaylar;

namespace Services.EkTanımlamalar
{
    public class HariciSektorServisi : IHariciSektorServisi
    {
        private const string HARICISEKTOR_ALL_KEY = "haricisektor.all-{0}-{1}";
        private const string HARICISEKTOR_BY_ID_KEY = "haricisektor.id-{0}";
        private const string HARICISEKTOR_PATTERN_KEY = "haricisektor.";
        private readonly IDepo<HariciSektor> _hariciDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public HariciSektorServisi(IDepo<HariciSektor> hariciDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._hariciDepo = hariciDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public HariciSektor HariciSektorAlId(int hariciId)
        {
            if (hariciId == 0)
                return null;

            string key = string.Format(HARICISEKTOR_BY_ID_KEY, hariciId);
            return _önbellekYönetici.Al(key, () => _hariciDepo.AlId(hariciId));
        }

        public void HariciSektorEkle(HariciSektor harici)
        {
            if (harici == null)
                throw new ArgumentNullException("harici");

            _hariciDepo.Ekle(harici);
            _önbellekYönetici.KalıpİleSil(HARICISEKTOR_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(harici);
        }

        public void HariciSektorGüncelle(HariciSektor harici)
        {
            if (harici == null)
                throw new ArgumentNullException("harici");

            _hariciDepo.Güncelle(harici);
            _önbellekYönetici.KalıpİleSil(HARICISEKTOR_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(harici);
        }

        public void HariciSektorSil(HariciSektor harici)
        {
            if (harici == null)
                throw new ArgumentNullException("harici");

            _hariciDepo.Sil(harici);
            _önbellekYönetici.KalıpİleSil(HARICISEKTOR_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(harici);
        }

        public IList<HariciSektor> TümHariciSektorleriAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(HARICISEKTOR_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _hariciDepo.Tablo;
                return query.ToList();
            });
        }
    }
}
