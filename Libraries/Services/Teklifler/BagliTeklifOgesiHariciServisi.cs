using Core;
using Core.Data;
using Core.Domain.Teklif;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Teklifler
{
    public class BagliTeklifOgesiHariciServisi : IBagliTeklifOgesiHariciServisi
    {
        private const string BAGLITEKLIFOGESI_ALL_KEY = "bagliTeklifOgesi.all-{0}-{1}";
        private const string BAGLITEKLIFOGESI_BY_ID_KEY = "bagliTeklifOgesi.id-{0}";
        private const string BAGLITEKLIFOGESI_PATTERN_KEY = "bagliTeklifOgesi.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<BagliTeklifOgesiHarici> _bagliTeklifOgesiDepo;
        private readonly ITeklifHariciServisi _teklifServisi;
        private readonly IDepo<TeklifHarici> _teklifDepo;
        public BagliTeklifOgesiHariciServisi(
            IDepo<BagliTeklifOgesiHarici> bagliTeklifOgesiDepo,
            IDepo<TeklifHarici> teklifDepo,
            ITeklifHariciServisi teklifServisi,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._bagliTeklifOgesiDepo = bagliTeklifOgesiDepo;
            this._teklifDepo = teklifDepo;
            this._teklifServisi = teklifServisi;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public BagliTeklifOgesiHarici BagliTeklifOgesiAlId(int bagliTeklifOgesiId)
        {
            if (bagliTeklifOgesiId == 0)
                return null;

            string key = string.Format(BAGLITEKLIFOGESI_BY_ID_KEY, bagliTeklifOgesiId);
            return _önbellekYönetici.Al(key, () => _bagliTeklifOgesiDepo.AlId(bagliTeklifOgesiId));
        }

        public void BagliTeklifOgesiEkle(BagliTeklifOgesiHarici bagliTeklifOgesi)
        {
            if (bagliTeklifOgesi == null)
                throw new ArgumentNullException("bagliTeklifOgesi");

            _bagliTeklifOgesiDepo.Ekle(bagliTeklifOgesi);
            _önbellekYönetici.KalıpİleSil(BAGLITEKLIFOGESI_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(bagliTeklifOgesi);
        }

        public void BagliTeklifOgesiGüncelle(BagliTeklifOgesiHarici bagliTeklifOgesi)
        {
            if (bagliTeklifOgesi == null)
                throw new ArgumentNullException("bagliTeklifOgesi");

            _bagliTeklifOgesiDepo.Güncelle(bagliTeklifOgesi);
            _önbellekYönetici.KalıpİleSil(BAGLITEKLIFOGESI_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(bagliTeklifOgesi);
        }

        public void BagliTeklifOgesiSil(BagliTeklifOgesiHarici bagliTeklifOgesi)
        {
            if (bagliTeklifOgesi == null)
                throw new ArgumentNullException("bagliTeklifOgesi");

            _bagliTeklifOgesiDepo.Sil(bagliTeklifOgesi);
            _önbellekYönetici.KalıpİleSil(BAGLITEKLIFOGESI_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(bagliTeklifOgesi);
        }

        public IList<BagliTeklifOgesiHarici> TümBagliTeklifOgesiAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(BAGLITEKLIFOGESI_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _bagliTeklifOgesiDepo.Tablo;
                return query.ToList();
            });
        }

        public IList<BagliTeklifOgesiHarici> BagliTeklifOgesiAlTeklifId(int teklifId )
        {
            if (teklifId == 0)
                return null;
            string key = string.Format(BAGLITEKLIFOGESI_BY_ID_KEY,teklifId);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _bagliTeklifOgesiDepo.Tablo.Where(x => x.TeklifId == teklifId).OrderBy(x => x.Vparent);
                return query.ToList();
            });
        }
        public IList<BagliTeklifOgesiHarici> BagliTeklifOgesiAlTeklifId(int teklifId, string durumu )
        {
            if (teklifId == 0)
                return null;
            teklifId = _teklifServisi.TeklifAlId(teklifId).Id;
            string key = string.Format(BAGLITEKLIFOGESI_BY_ID_KEY, teklifId);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _bagliTeklifOgesiDepo.Tablo.Where(x => x.TeklifId == teklifId).OrderBy(x => x.Vparent);
                return query.ToList();
            });
        }
    }

}
