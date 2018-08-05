using Core;
using Core.Data;
using Core.Domain.Finans;
using Core.Domain.Kullanıcılar;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace Services.Finans
{
    public class OdemeFormuServisi : IOdemeFormuServisi
    {
        private const string ODEMEFORMU_ALL_KEY = "odemeFormu.all-{0}-{1}";
        private const string ODEMEFORMU_BY_ID_KEY = "odemeFormu.id-{0}";
        private const string ODEMEFORMU_PATTERN_KEY = "odemeFormu.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<OdemeFormu> _odemeFormuDepo;
        public OdemeFormuServisi(IDepo<OdemeFormu> odemeFormuDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._odemeFormuDepo = odemeFormuDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public OdemeFormu OdemeFormuAlId(int odemeFormuId)
        {
            if (odemeFormuId == 0)
                return null;

            string key = string.Format(ODEMEFORMU_BY_ID_KEY, odemeFormuId);
            return _önbellekYönetici.Al(key, () => _odemeFormuDepo.AlId(odemeFormuId));
        }

        public void OdemeFormuEkle(OdemeFormu odemeFormu)
        {
            if (odemeFormu == null)
                throw new ArgumentNullException("odemeFormu");

            _odemeFormuDepo.Ekle(odemeFormu);
            _önbellekYönetici.KalıpİleSil(ODEMEFORMU_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(odemeFormu);
        }

        public void OdemeFormuGüncelle(OdemeFormu odemeFormu)
        {
            if (odemeFormu == null)
                throw new ArgumentNullException("odemeFormu");

            _odemeFormuDepo.Güncelle(odemeFormu);
            _önbellekYönetici.KalıpİleSil(ODEMEFORMU_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(odemeFormu);
        }

        public void OdemeFormuSil(OdemeFormu odemeFormu)
        {
            if (odemeFormu == null)
                throw new ArgumentNullException("odemeFormu");

            _odemeFormuDepo.Sil(odemeFormu);
            _önbellekYönetici.KalıpİleSil(ODEMEFORMU_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(odemeFormu);
        }

        public IList<OdemeFormu> TümOdemeFormuAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            string key = string.Format(ODEMEFORMU_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _odemeFormuDepo.Tablo;
                query = query.OrderByDescending(x => x.Id);
                return query.ToList(); 
            });
        }

        public ISayfalıListe<OdemeFormu> OdemeFormuAra(string firma, 
           int kongreGunu,int kongreAyı, int odemeGunu,int odemeAyı, string aciklama,string alisFatura,string satisFatura,
           bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _odemeFormuDepo.Tablo;
            if (!String.IsNullOrEmpty(firma))
                sorgu = sorgu.Where(qe => qe.Firma.Contains(firma));
            
            if (!String.IsNullOrEmpty(aciklama))
                sorgu = sorgu.Where(qe => qe.Aciklama.Contains(aciklama));
            if (!String.IsNullOrEmpty(alisFatura))
                sorgu = sorgu.Where(qe => qe.FaturaNo.Contains(alisFatura));
            if (!String.IsNullOrEmpty(satisFatura))
                sorgu = sorgu.Where(qe => qe.SatisFaturaNo.Contains(satisFatura));
            if (kongreGunu > 0 && kongreAyı > 0)
            {
                sorgu = sorgu.Where(x => x.KongreTarihi.Day == kongreGunu && x.KongreTarihi.Month == kongreAyı);
            }
            else if (kongreGunu > 0)
            {
                sorgu = sorgu.Where(x => x.KongreTarihi.Day == kongreGunu);

            }
            else if (kongreAyı > 0)
            {
                sorgu = sorgu.Where(x => x.KongreTarihi.Month == kongreAyı);
            }

            if (odemeGunu > 0 && odemeAyı > 0)
            {
                sorgu = sorgu.Where(x => x.OdemeTarihi.Day == odemeGunu && x.OdemeTarihi.Month == odemeAyı);
            }
            else if (odemeGunu > 0)
            {
                sorgu = sorgu.Where(x => x.OdemeTarihi.Day == odemeGunu);

            }
            else if (odemeAyı > 0)
            {
                sorgu = sorgu.Where(x => x.OdemeTarihi.Month == odemeAyı);
            }
            sorgu = enYeniler ?
                sorgu.OrderByDescending(qe => qe.OdemeTarihi) :
                sorgu.OrderByDescending(qe => qe.OdemeTarihi).ThenBy(qe => qe.OdemeTarihi);
            sorgu = sorgu.OrderByDescending(qe => qe.Id);

            var odemeFormları = new SayfalıListe<OdemeFormu>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return odemeFormları;
        }
    }

}
