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
    public class YDAcenteServisi : IYDAcenteServisi
    {
        private const string YDACENTE_ALL_KEY = "yDAcente.all-{0}-{1}";
        private const string YDACENTE_BY_ID_KEY = "yDAcente.id-{0}";
        private const string YDACENTE_PATTERN_KEY = "yDAcente.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<YDAcente> _yDAcenteDepo;
        public YDAcenteServisi(IDepo<YDAcente> yDAcenteDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._yDAcenteDepo = yDAcenteDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public YDAcente YDAcenteAlId(int yDAcenteId)
        {
            if (yDAcenteId == 0)
                return null;

            string key = string.Format(YDACENTE_BY_ID_KEY, yDAcenteId);
            return _önbellekYönetici.Al(key, () => _yDAcenteDepo.AlId(yDAcenteId));
        }

        public void YDAcenteEkle(YDAcente yDAcente)
        {
            if (yDAcente == null)
                throw new ArgumentNullException("yDAcente");

            _yDAcenteDepo.Ekle(yDAcente);
            _önbellekYönetici.KalıpİleSil(YDACENTE_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(yDAcente);
        }

        public void YDAcenteGüncelle(YDAcente yDAcente)
        {
            if (yDAcente == null)
                throw new ArgumentNullException("yDAcente");

            _yDAcenteDepo.Güncelle(yDAcente);
            _önbellekYönetici.KalıpİleSil(YDACENTE_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(yDAcente);
        }

        public void YDAcenteSil(YDAcente yDAcente)
        {
            if (yDAcente == null)
                throw new ArgumentNullException("yDAcente");

            _yDAcenteDepo.Sil(yDAcente);
            _önbellekYönetici.KalıpİleSil(YDACENTE_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(yDAcente);
        }

        public IList<YDAcente> TümYDAcenteAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(YDACENTE_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _yDAcenteDepo.Tablo;
                return query.ToList();
            });
        }
    }
}
