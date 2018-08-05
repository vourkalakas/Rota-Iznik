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
    public class UnvanlarServisi : IUnvanlarServisi
    {
        private const string UNVANLAR_ALL_KEY = "ünvanlar.all-{0}-{1}";
        private const string UNVANLAR_BY_ID_KEY = "ünvanlar.id-{0}";
        private const string UNVANLAR_PATTERN_KEY = "ünvanlar.";
        private readonly IDepo<Unvanlar> _ünvanlarDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public UnvanlarServisi(IDepo<Unvanlar> ünvanlarDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._ünvanlarDepo = ünvanlarDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Unvanlar UnvanlarAlId(int ünvanlarId)
        {
            if (ünvanlarId == 0)
                return null;

            string key = string.Format(UNVANLAR_BY_ID_KEY, ünvanlarId);
            return _önbellekYönetici.Al(key, () => _ünvanlarDepo.AlId(ünvanlarId));
        }

        public void UnvanlarEkle(Unvanlar Unvanlar)
        {
            if (Unvanlar == null)
                throw new ArgumentNullException("Unvanlar");

            _ünvanlarDepo.Ekle(Unvanlar);
            _önbellekYönetici.KalıpİleSil(UNVANLAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(Unvanlar);
        }

        public void UnvanlarGüncelle(Unvanlar Unvanlar)
        {
            if (Unvanlar == null)
                throw new ArgumentNullException("Unvanlar");

            _ünvanlarDepo.Güncelle(Unvanlar);
            _önbellekYönetici.KalıpİleSil(UNVANLAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(Unvanlar);
        }

        public void UnvanlarSil(Unvanlar Unvanlar)
        {
            if (Unvanlar == null)
                throw new ArgumentNullException("Unvanlar");

            _ünvanlarDepo.Sil(Unvanlar);
            _önbellekYönetici.KalıpİleSil(UNVANLAR_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(Unvanlar);
        }

        public IList<Unvanlar> TümUnvanlarıAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(UNVANLAR_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _ünvanlarDepo.Tablo;
                return query.ToList();
            });
        }
    }
}
