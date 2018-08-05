using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Core.Domain.EkTanımlamalar;
using Core.Önbellek;
using Services.Olaylar;

namespace Services.EkTanımlamalar
{
    public class TeklifKalemiServisi : ITeklifKalemiServisi
    {
        private const string TEKLIFKALEMI_ALL_KEY = "teklifkalemi.all-{0}-{1}";
        private const string TEKLIFKALEMI_BY_ID_KEY = "teklifkalemi.id-{0}";
        private const string TEKLIFKALEMI_PATTERN_KEY = "teklifkalemi.";
        private readonly IDepo<TeklifKalemi> _teklifkalemiDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public TeklifKalemiServisi(IDepo<TeklifKalemi> teklifkalemiDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._teklifkalemiDepo = teklifkalemiDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public TeklifKalemi TeklifKalemiAlId(int teklifkalemiId)
        {
            if (teklifkalemiId == 0)
                return null;

            string key = string.Format(TEKLIFKALEMI_BY_ID_KEY, teklifkalemiId);
            return _önbellekYönetici.Al(key, () => _teklifkalemiDepo.AlId(teklifkalemiId));
        }

        public void TeklifKalemiEkle(TeklifKalemi teklifkalemi)
        {
            if (teklifkalemi == null)
                throw new ArgumentNullException("teklifkalemi");

            _teklifkalemiDepo.Ekle(teklifkalemi);
            _önbellekYönetici.KalıpİleSil(TEKLIFKALEMI_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(teklifkalemi);
        }

        public void TeklifKalemiGüncelle(TeklifKalemi teklifkalemi)
        {
            if (teklifkalemi == null)
                throw new ArgumentNullException("teklifkalemi");

            _teklifkalemiDepo.Güncelle(teklifkalemi);
            _önbellekYönetici.KalıpİleSil(TEKLIFKALEMI_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(teklifkalemi);
        }

        public void TeklifKalemiSil(TeklifKalemi teklifkalemi)
        {
            if (teklifkalemi == null)
                throw new ArgumentNullException("teklifkalemi");

            _teklifkalemiDepo.Sil(teklifkalemi);
            _önbellekYönetici.KalıpİleSil(TEKLIFKALEMI_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(teklifkalemi);
        }

        public IList<TeklifKalemi> TümTeklifKalemleriAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TEKLIFKALEMI_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _teklifkalemiDepo.Tablo.OrderBy(x => x.SıraNo);
                return query.ToList();
            });
        }
        public IList<TeklifKalemi> AnaTeklifKalemleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TEKLIFKALEMI_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
             {
                 var query = from ft in _teklifkalemiDepo.Tablo
                             where ft.NodeId.HasValue == false
                             orderby ft.SıraNo ascending
                             select ft;
                 return query.ToList();
             });
        }
    }
}
