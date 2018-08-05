using Core;
using Core.Data;
using Core.Domain.Kongre;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Kongre
{
    public class KursServisi : IKursServisi
    {
        private const string KURS_ALL_KEY = "kurs.all-{0}-{1}";
        private const string KURS_BY_ID_KEY = "kurs.id-{0}";
        private const string KURS_PATTERN_KEY = "kurs.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Kurs> _kursDepo;
        public KursServisi(IDepo<Kurs> kursDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._kursDepo = kursDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Kurs KursAlId(int kursId)
        {
            if (kursId == 0)
                return null;

            string key = string.Format(KURS_BY_ID_KEY, kursId);
            return _önbellekYönetici.Al(key, () => _kursDepo.AlId(kursId));
        }

        public void KursEkle(Kurs kurs)
        {
            if (kurs == null)
                throw new ArgumentNullException("kurs");

            _kursDepo.Ekle(kurs);
            _önbellekYönetici.KalıpİleSil(KURS_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(kurs);
        }

        public void KursGüncelle(Kurs kurs)
        {
            if (kurs == null)
                throw new ArgumentNullException("kurs");

            _kursDepo.Güncelle(kurs);
            _önbellekYönetici.KalıpİleSil(KURS_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(kurs);
        }

        public void KursSil(Kurs kurs)
        {
            if (kurs == null)
                throw new ArgumentNullException("kurs");

            _kursDepo.Sil(kurs);
            _önbellekYönetici.KalıpİleSil(KURS_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(kurs);
        }

        public IList<Kurs> TümKursAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(KURS_ALL_KEY,  AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _kursDepo.Tablo;
                return query.ToList();
            });
        }
        public IList<Kurs> KursAlKongreId(int kongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(KURS_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _kursDepo.Tablo.Where(x => x.KongreId == kongreId);
                return query.ToList();
            });
        }
    }

}
