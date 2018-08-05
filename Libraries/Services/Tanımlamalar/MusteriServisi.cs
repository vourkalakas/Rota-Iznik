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
    public class MusteriServisi : IMusteriServisi
    {
        private const string MUSTERİ_ALL_KEY = "musteri.all-{0}-{1}";
        private const string MUSTERİ_BY_ID_KEY = "musteri.id-{0}";
        private const string MUSTERİ_PATTERN_KEY = "musteri.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Musteri> _musteriDepo;
        public MusteriServisi(IDepo<Musteri> musteriDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._musteriDepo = musteriDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Musteri MusteriAlId(int musteriId)
        {
            if (musteriId == 0)
                return null;

            string key = string.Format(MUSTERİ_BY_ID_KEY, musteriId);
            return _önbellekYönetici.Al(key, () => _musteriDepo.AlId(musteriId));
        }

        public void MusteriEkle(Musteri musteri)
        {
            if (musteri == null)
                throw new ArgumentNullException("musteri");

            _musteriDepo.Ekle(musteri);
            _önbellekYönetici.KalıpİleSil(MUSTERİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(musteri);
        }

        public void MusteriGüncelle(Musteri musteri)
        {
            if (musteri == null)
                throw new ArgumentNullException("musteri");

            _musteriDepo.Güncelle(musteri);
            _önbellekYönetici.KalıpİleSil(MUSTERİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(musteri);
        }

        public void MusteriSil(Musteri musteri)
        {
            if (musteri == null)
                throw new ArgumentNullException("musteri");

            _musteriDepo.Sil(musteri);
            _önbellekYönetici.KalıpİleSil(MUSTERİ_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(musteri);
        }

        public IList<Musteri> TümMusteriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(MUSTERİ_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _musteriDepo.Tablo;
                return query.ToList();
            });
        }
    }
}
