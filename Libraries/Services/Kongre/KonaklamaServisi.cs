using Core;
using Core.Data;
using Core.Domain.Kongre;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Kongre
{
    public class KonaklamaServisi : IKonaklamaServisi
    {
        private const string KONAKLAMA_ALL_KEY = "konaklama.all-{0}-{1}";
        private const string KONAKLAMA_BY_ID_KEY = "konaklama.id-{0}";
        private const string KONAKLAMA_PATTERN_KEY = "konaklama.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Konaklama> _konaklamaDepo;
        public KonaklamaServisi(IDepo<Konaklama> konaklamaDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._konaklamaDepo = konaklamaDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Konaklama KonaklamaAlId(int konaklamaId)
        {
            if (konaklamaId == 0)
                return null;

            string key = string.Format(KONAKLAMA_BY_ID_KEY, konaklamaId);
            return _önbellekYönetici.Al(key, () => _konaklamaDepo.AlId(konaklamaId));
        }

        public void KonaklamaEkle(Konaklama konaklama)
        {
            if (konaklama == null)
                throw new ArgumentNullException("konaklama");

            _konaklamaDepo.Ekle(konaklama);
            _önbellekYönetici.KalıpİleSil(KONAKLAMA_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(konaklama);
        }

        public void KonaklamaGüncelle(Konaklama konaklama)
        {
            if (konaklama == null)
                throw new ArgumentNullException("konaklama");

            _konaklamaDepo.Güncelle(konaklama);
            _önbellekYönetici.KalıpİleSil(KONAKLAMA_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(konaklama);
        }

        public void KonaklamaSil(Konaklama konaklama)
        {
            if (konaklama == null)
                throw new ArgumentNullException("konaklama");

            _konaklamaDepo.Sil(konaklama);
            _önbellekYönetici.KalıpİleSil(KONAKLAMA_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(konaklama);
        }

        public IList<Konaklama> TümKonaklamaAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
           
                var query = _konaklamaDepo.Tablo;
                return query.ToList();
            
        }
        public IList<Konaklama> KonaklamaAlKongreId(int kongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(KONAKLAMA_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _konaklamaDepo.Tablo.Where(x => x.KongreId == kongreId);
                return query.ToList();
            });
        }
    }

}
