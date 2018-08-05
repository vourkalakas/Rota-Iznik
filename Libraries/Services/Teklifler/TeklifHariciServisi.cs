using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Data;
using Core.Domain.Teklif;
using Core.Önbellek;
using Services.Olaylar;

namespace Services.Teklifler
{
    public class TeklifHariciServisi : ITeklifHariciServisi
    {
        private const string TEKLIF_ALL_KEY = "teklif.all-{0}-{1}";
        private const string TEKLIF_BY_ID_KEY = "teklif.id-{0}";
        private const string TEKLIF_PATTERN_KEY = "teklif.";
        private readonly IDepo<TeklifHarici> _teklifDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public TeklifHariciServisi(IDepo<TeklifHarici> teklifDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._teklifDepo = teklifDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public TeklifHarici TeklifAlId(int teklifId)
        {
            if (teklifId == 0)
                return null;

            string key = string.Format(TEKLIF_BY_ID_KEY, teklifId);
            return _önbellekYönetici.Al(key, () => _teklifDepo.AlId(teklifId));
        }

        public void TeklifEkle(TeklifHarici teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");

            _teklifDepo.Ekle(teklif);
            _önbellekYönetici.KalıpİleSil(TEKLIF_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(teklif);
        }

        public void TeklifGüncelle(TeklifHarici teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");

            _teklifDepo.Güncelle(teklif);
            _önbellekYönetici.KalıpİleSil(TEKLIF_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(teklif);
        }

        public void TeklifSil(TeklifHarici teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");

            _teklifDepo.Sil(teklif);
            _önbellekYönetici.KalıpİleSil(TEKLIF_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(teklif);
        }

        public IList<TeklifHarici> TümTeklifAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TEKLIF_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _teklifDepo.Tablo;
                query = query.OrderByDescending(x => x.Tarih);
                return query.ToList();
            });
        }

        public ISayfalıListe<TeklifHarici> TeklifAra(string adı, string acenta, string po, string talepno,
           DateTime? tarihi, DateTime? teslimTarihi,bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _teklifDepo.Tablo;
            if (!String.IsNullOrEmpty(adı))
                sorgu = sorgu.Where(qe => qe.Adı.Contains(adı));
            if (!String.IsNullOrEmpty(acenta))
                sorgu = sorgu.Where(qe => qe.Acenta.Contains(acenta));
            if (tarihi.HasValue)
                sorgu = sorgu.Where(qe => qe.Tarih == tarihi);
            if (!String.IsNullOrEmpty(po))
                sorgu = sorgu.Where(qe => qe.Po.Contains(po));
            if (!String.IsNullOrEmpty(talepno))
                sorgu = sorgu.Where(qe => qe.TalepNo.Contains(talepno));
            if (tarihi.HasValue)
                if (teslimTarihi.HasValue)
                sorgu = sorgu.Where(qe => qe.TeslimTarihi == teslimTarihi);
            sorgu = enYeniler ?
                sorgu.OrderByDescending(qe => qe.Tarih) :
                sorgu.OrderByDescending(qe => qe.Tarih).ThenBy(qe => qe.Tarih);

            var teklifler = new SayfalıListe<TeklifHarici>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return teklifler;
        }
    }
}
