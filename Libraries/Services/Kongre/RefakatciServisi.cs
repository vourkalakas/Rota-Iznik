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
    public class RefakatciServisi : IRefakatciServisi
    {
        private const string KATİLİMCİ_ALL_KEY = "refakatci.all-{0}-{1}";
        private const string KATİLİMCİ_BY_ID_KEY = "refakatci.id-{0}";
        private const string KATİLİMCİ_PATTERN_KEY = "refakatci.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Refakatci> _refakatciDepo;
        public RefakatciServisi(IDepo<Refakatci> refakatciDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._refakatciDepo = refakatciDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Refakatci RefakatciAlId(int refakatciId)
        {
            if (refakatciId == 0)
                return null;

            string key = string.Format(KATİLİMCİ_BY_ID_KEY, refakatciId);
            return _önbellekYönetici.Al(key, () => _refakatciDepo.AlId(refakatciId));
        }

        public void RefakatciEkle(Refakatci refakatci)
        {
            if (refakatci == null)
                throw new ArgumentNullException("refakatci");

            _refakatciDepo.Ekle(refakatci);
            _önbellekYönetici.KalıpİleSil(KATİLİMCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(refakatci);
        }

        public void RefakatciGüncelle(Refakatci refakatci)
        {
            if (refakatci == null)
                throw new ArgumentNullException("refakatci");

            _refakatciDepo.Güncelle(refakatci);
            _önbellekYönetici.KalıpİleSil(KATİLİMCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(refakatci);
        }

        public void RefakatciSil(Refakatci refakatci)
        {
            if (refakatci == null)
                throw new ArgumentNullException("refakatci");

            _refakatciDepo.Sil(refakatci);
            _önbellekYönetici.KalıpİleSil(KATİLİMCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(refakatci);
        }

        public IList<Refakatci> TümRefakatciAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(KATİLİMCİ_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _refakatciDepo.Tablo;
                return query.ToList();
            });
        }
    }

}
