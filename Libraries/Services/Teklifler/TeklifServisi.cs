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
    public class TeklifServisi : ITeklifServisi
    {
        private const string TEKLIF_ALL_KEY = "teklif.all-{0}-{1}";
        private const string TEKLIF_BY_ID_KEY = "teklif.id-{0}";
        private const string TEKLIF_PATTERN_KEY = "teklif.";
        private readonly IDepo<Teklif> _teklifDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public TeklifServisi(IDepo<Teklif> teklifDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._teklifDepo = teklifDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Teklif TeklifAlId(int teklifId)
        {
            if (teklifId == 0)
                return null;

            string key = string.Format(TEKLIF_BY_ID_KEY, teklifId);
            return _önbellekYönetici.Al(key, () => _teklifDepo.AlId(teklifId));
        }

        public void TeklifEkle(Teklif teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");
            _teklifDepo.Ekle(teklif);
            _önbellekYönetici.KalıpİleSil(TEKLIF_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(teklif);
        }

        public void TeklifGüncelle(Teklif teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");
            teklif.OlusturulmaTarihi = DateTime.Now;
            _teklifDepo.Güncelle(teklif);
            _önbellekYönetici.KalıpİleSil(TEKLIF_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(teklif);
        }

        public void TeklifSil(Teklif teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");

            _teklifDepo.Sil(teklif);
            _önbellekYönetici.KalıpİleSil(TEKLIF_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(teklif);
        }

        public IList<Teklif> TümTeklifAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TEKLIF_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _teklifDepo.Tablo;
                query = query.OrderByDescending(x => x.Id);
                return query.ToList();
            });
        }
        public ISayfalıListe<Teklif> TeklifAra(DateTime? tarihinden = null,
            DateTime? tarihine = null, int hazırlayanId = 0, string adı = "",
            string Konumu = "", string açıklama = "", string durumu = "", 
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _teklifDepo.Tablo;
            if (tarihinden.HasValue)
                sorgu = sorgu.Where(c => tarihinden.Value <= c.OlusturulmaTarihi);
            if (tarihine.HasValue)
                sorgu = sorgu.Where(c => tarihine.Value >= c.OlusturulmaTarihi);
            if (!String.IsNullOrEmpty(adı))
                sorgu = sorgu.Where(qe => qe.Adı.Contains(adı));
            if (hazırlayanId>0)
                sorgu = sorgu.Where(qe => qe.HazırlayanId == hazırlayanId);
            if (!String.IsNullOrEmpty(Konumu))
                sorgu = sorgu.Where(qe => qe.Konum.Contains(Konumu));
            if (!String.IsNullOrEmpty(açıklama))
                sorgu = sorgu.Where(qe => qe.Aciklama.Contains(açıklama));
            if (durumu != null && durumu.Length > 0)
                sorgu = sorgu.Where(c => c.Durumu.Contains(durumu));

            var teklifler = new SayfalıListe<Teklif>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return teklifler;
        }
    }
}
