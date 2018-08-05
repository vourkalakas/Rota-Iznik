using Core;
using Core.Data;
using Core.Domain.Tanımlamalar;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Tanımlamalar
{
    public class OtelServisi : IOtelServisi
    {
        private const string OTEL_ALL_KEY = "otel.all-{0}-{1}";
        private const string OTEL_BY_ID_KEY = "otel.id-{0}";
        private const string OTEL_PATTERN_KEY = "otel.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Otel> _otelDepo;
        public OtelServisi(IDepo<Otel> otelDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._otelDepo = otelDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Otel OtelAlId(int otelId)
        {
            if (otelId == 0)
                return null;

            string key = string.Format(OTEL_BY_ID_KEY, otelId);
            return _önbellekYönetici.Al(key, () => _otelDepo.AlId(otelId));
        }

        public void OtelEkle(Otel otel)
        {
            if (otel == null)
                throw new ArgumentNullException("otel");

            _otelDepo.Ekle(otel);
            _önbellekYönetici.KalıpİleSil(OTEL_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(otel);
        }

        public void OtelGüncelle(Otel otel)
        {
            if (otel == null)
                throw new ArgumentNullException("otel");

            _otelDepo.Güncelle(otel);
            _önbellekYönetici.KalıpİleSil(OTEL_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(otel);
        }

        public void OtelSil(Otel otel)
        {
            if (otel == null)
                throw new ArgumentNullException("otel");

            _otelDepo.Sil(otel);
            _önbellekYönetici.KalıpİleSil(OTEL_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(otel);
        }

        public IList<Otel> TümOtelAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(OTEL_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _otelDepo.Tablo;
                return query.ToList();
            });
        }
    }

}
