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
    public class KontenjanServisi : IKontenjanServisi
    {
        private const string Kontenjan_ALL_KEY = "Kontenjan.all-{0}-{1}";
        private const string Kontenjan_BY_ID_KEY = "Kontenjan.id-{0}";
        private const string Kontenjan_PATTERN_KEY = "Kontenjan.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Kontenjan> _KontenjanDepo;
        public KontenjanServisi(IDepo<Kontenjan> KontenjanDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._KontenjanDepo = KontenjanDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Kontenjan KontenjanAlId(int KontenjanId)
        {
            if (KontenjanId == 0)
                return null;

            string key = string.Format(Kontenjan_BY_ID_KEY, KontenjanId);
            return _önbellekYönetici.Al(key, () => _KontenjanDepo.AlId(KontenjanId));
        }
        public IList<Kontenjan> KontenjanAlKongreId(int KongreId)
        {
            if (KongreId == 0)
                return null;

            var query = _KontenjanDepo.Tablo.Where(x => x.KongreId == KongreId);
            return query.ToList();
        }

        public void KontenjanEkle(Kontenjan Kontenjan)
        {
            if (Kontenjan == null)
                throw new ArgumentNullException("Kontenjan");

            _KontenjanDepo.Ekle(Kontenjan);
            _önbellekYönetici.KalıpİleSil(Kontenjan_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(Kontenjan);
        }

        public void KontenjanGüncelle(Kontenjan Kontenjan)
        {
            if (Kontenjan == null)
                throw new ArgumentNullException("Kontenjan");

            _KontenjanDepo.Güncelle(Kontenjan);
            _önbellekYönetici.KalıpİleSil(Kontenjan_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(Kontenjan);
        }

        public void KontenjanSil(Kontenjan Kontenjan)
        {
            if (Kontenjan == null)
                throw new ArgumentNullException("Kontenjan");

            _KontenjanDepo.Sil(Kontenjan);
            _önbellekYönetici.KalıpİleSil(Kontenjan_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(Kontenjan);
        }

        public IList<Kontenjan> TümKontenjanAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            var query = _KontenjanDepo.Tablo;
            return query.ToList();
        }
    }
}


