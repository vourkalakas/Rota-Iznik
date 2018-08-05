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
    public class KongreServisi : IKongreServisi
    {
        private const string KONGRELER_ALL_KEY = "kongreler.all";
        private const string KONGRELER_BY_ID_KEY = "kongreler.id-{0}";
        private const string KONGRELER_PATTERN_KEY = "kongreler.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Kongreler> _kongrelerDepo;
        public KongreServisi(IDepo<Kongreler> kongrelerDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._kongrelerDepo = kongrelerDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Kongreler KongrelerAlId(int kongrelerId)
        {
            if (kongrelerId == 0)
                return null;

            string key = string.Format(KONGRELER_BY_ID_KEY, kongrelerId);
            return _önbellekYönetici.Al(key, () => _kongrelerDepo.AlId(kongrelerId));
        }

        public void KongrelerEkle(Kongreler kongreler)
        {
            if (kongreler == null)
                throw new ArgumentNullException("kongreler");

            _kongrelerDepo.Ekle(kongreler);
            _önbellekYönetici.KalıpİleSil(KONGRELER_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(kongreler);
        }

        public void KongrelerGüncelle(Kongreler kongreler)
        {
            if (kongreler == null)
                throw new ArgumentNullException("kongreler");

            _kongrelerDepo.Güncelle(kongreler);
            _önbellekYönetici.KalıpİleSil(KONGRELER_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(kongreler);
        }

        public void KongrelerSil(Kongreler kongreler)
        {
            if (kongreler == null)
                throw new ArgumentNullException("kongreler");

            _kongrelerDepo.Sil(kongreler);
            _önbellekYönetici.KalıpİleSil(KONGRELER_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(kongreler);
        }

        public IList<Kongreler> TümKongrelerAl()
        {
            string key = string.Format(KONGRELER_ALL_KEY);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _kongrelerDepo.Tablo;
                return query.ToList();
            });
        }
        public ISayfalıListe<Kongreler> TümKongrelerAl(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var sorgu = _kongrelerDepo.Tablo;
            sorgu = sorgu.OrderBy(pr => pr.BaslamaTarihi).ThenBy(pr => pr.Id);
            var tümKongreler = new SayfalıListe<Kongreler>(sorgu, pageIndex, pageSize);
            return tümKongreler;
        }
    }

}
