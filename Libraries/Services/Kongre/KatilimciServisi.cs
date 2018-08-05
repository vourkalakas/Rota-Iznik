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
    public class KatilimciServisi : IKatilimciServisi
    {
        private const string KATİLİMCİ_ALL_KEY = "katilimci.all-{0}-{1}";
        private const string KATİLİMCİ_BY_ID_KEY = "katilimci.id-{0}";
        private const string KATİLİMCİ_PATTERN_KEY = "katilimci.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Katilimci> _katilimciDepo;
        public KatilimciServisi(IDepo<Katilimci> katilimciDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._katilimciDepo = katilimciDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Katilimci KatilimciAlId(int katilimciId)
        {
            if (katilimciId == 0)
                return null;

            string key = string.Format(KATİLİMCİ_BY_ID_KEY, katilimciId);
            return _önbellekYönetici.Al(key, () => _katilimciDepo.AlId(katilimciId));
        }

        public void KatilimciEkle(Katilimci katilimci)
        {
            if (katilimci == null)
                throw new ArgumentNullException("katilimci");

            _katilimciDepo.Ekle(katilimci);
            _önbellekYönetici.KalıpİleSil(KATİLİMCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(katilimci);
        }

        public void KatilimciGüncelle(Katilimci katilimci)
        {
            if (katilimci == null)
                throw new ArgumentNullException("katilimci");

            _katilimciDepo.Güncelle(katilimci);
            _önbellekYönetici.KalıpİleSil(KATİLİMCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(katilimci);
        }

        public void KatilimciSil(Katilimci katilimci)
        {
            if (katilimci == null)
                throw new ArgumentNullException("katilimci");

            _katilimciDepo.Sil(katilimci);
            _önbellekYönetici.KalıpİleSil(KATİLİMCİ_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(katilimci);
        }

        public IList<Katilimci> TümKatilimciAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            
                var query = _katilimciDepo.Tablo;
                return query.ToList();
            
        }

        public ISayfalıListe<Katilimci> KatılımcıAra(int kongreId, string katılımcıAdı,
           bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _katilimciDepo.Tablo;
            if (kongreId>0)
                sorgu = sorgu.Where(qe => qe.KongreId==kongreId);

            if (!String.IsNullOrEmpty(katılımcıAdı))
                sorgu = sorgu.Where(qe => qe.Adı.Contains(katılımcıAdı));
            sorgu = sorgu.OrderByDescending(qe => qe.Id);
            var katılımcılıar = new SayfalıListe<Katilimci>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return katılımcılıar;
        }
    }
}


