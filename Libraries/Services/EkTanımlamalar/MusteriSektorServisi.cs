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
    public class MusteriSektorServisi : IMusteriSektorServisi
    {
        private const string MUSTERI_ALL_KEY = "musteri.all-{0}-{1}";
        private const string MUSTERI_BY_ID_KEY = "musteri.id-{0}";
        private const string MUSTERI_PATTERN_KEY = "musteri.";
        private readonly IDepo<MusteriSektor> _musteriDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public MusteriSektorServisi(IDepo<MusteriSektor> musteriDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._musteriDepo = musteriDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public MusteriSektor MusteriAlId(int musteriId)
        {
            if (musteriId == 0)
                return null;

            string key = string.Format(MUSTERI_BY_ID_KEY, musteriId);
            return _önbellekYönetici.Al(key, () => _musteriDepo.AlId(musteriId));
        }

        public void MusteriEkle(MusteriSektor Musteri)
        {
            if (Musteri == null)
                throw new ArgumentNullException("MusteriSektor");

            _musteriDepo.Ekle(Musteri);
            _önbellekYönetici.KalıpİleSil(MUSTERI_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(Musteri);
        }

        public void MusteriGüncelle(MusteriSektor Musteri)
        {
            if (Musteri == null)
                throw new ArgumentNullException("MusteriSektor");

            _musteriDepo.Güncelle(Musteri);
            _önbellekYönetici.KalıpİleSil(MUSTERI_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(Musteri);
        }

        public void MusteriSil(MusteriSektor Musteri)
        {
            if (Musteri == null)
                throw new ArgumentNullException("Musteri");

            _musteriDepo.Sil(Musteri);
            _önbellekYönetici.KalıpİleSil(MUSTERI_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(Musteri);
        }

        public IList<MusteriSektor> TümMusterilarıAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(MUSTERI_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _musteriDepo.Tablo;
                return query.ToList();
            });
        }
    }
}
