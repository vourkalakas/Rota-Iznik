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
    public class TedarikciServisi : ITedarikciServisi
    {
        private const string TEDARİKCİ_ALL_KEY = "tedarikci.all-{0}-{1}";
        private const string TEDARİKCİ_BY_ID_KEY = "tedarikci.id-{0}";
        private const string TEDARİKCİ_PATTERN_KEY = "tedarikci.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Tedarikci> _tedarikciDepo;
        public TedarikciServisi(IDepo<Tedarikci> tedarikciDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._tedarikciDepo = tedarikciDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Tedarikci TedarikciAlId(int tedarikciId)
        {
            if (tedarikciId == 0)
                return null;

            string key = string.Format(TEDARİKCİ_BY_ID_KEY, tedarikciId);
            return _önbellekYönetici.Al(key, () => _tedarikciDepo.AlId(tedarikciId));
        }

        public void TedarikciEkle(Tedarikci tedarikci)
        {
            if (tedarikci == null)
                throw new ArgumentNullException("tedarikci");

            _tedarikciDepo.Ekle(tedarikci);
            _önbellekYönetici.KalıpİleSil(TEDARİKCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(tedarikci);
        }

        public void TedarikciGüncelle(Tedarikci tedarikci)
        {
            if (tedarikci == null)
                throw new ArgumentNullException("tedarikci");

            _tedarikciDepo.Güncelle(tedarikci);
            _önbellekYönetici.KalıpİleSil(TEDARİKCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(tedarikci);
        }

        public void TedarikciSil(Tedarikci tedarikci)
        {
            if (tedarikci == null)
                throw new ArgumentNullException("tedarikci");

            _tedarikciDepo.Sil(tedarikci);
            _önbellekYönetici.KalıpİleSil(TEDARİKCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(tedarikci);
        }

        public IList<Tedarikci> TümTedarikciAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TEDARİKCİ_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _tedarikciDepo.Tablo;
                return query.ToList();
            });
        }
    }
}
