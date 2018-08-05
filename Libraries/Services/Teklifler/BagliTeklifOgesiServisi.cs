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
    public class BagliTeklifOgesiServisi : IBagliTeklifOgesiServisi
    {
        private const string BAGLITEKLIFOGESI_ALL_KEY = "bagliTeklifOgesi.all-{0}-{1}";
        private const string BAGLITEKLIFOGESI_BY_ID_KEY = "bagliTeklifOgesi.id-{0}";
        private const string BAGLITEKLIFOGESI_PATTERN_KEY = "bagliTeklifOgesi.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<BagliTeklifOgesi> _bagliTeklifOgesiDepo;
        private readonly ITeklifServisi _teklifServisi;
        private readonly IDepo<Teklif> _teklifDepo;
        public BagliTeklifOgesiServisi(
            IDepo<BagliTeklifOgesi> bagliTeklifOgesiDepo,
            IDepo<Teklif> teklifDepo,
            ITeklifServisi teklifServisi,
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
        public BagliTeklifOgesi BagliTeklifOgesiAlId(int bagliTeklifOgesiId)
        {
            if (bagliTeklifOgesiId == 0)
                return null;

            string key = string.Format(BAGLITEKLIFOGESI_BY_ID_KEY, bagliTeklifOgesiId);
            return _önbellekYönetici.Al(key, () => _bagliTeklifOgesiDepo.AlId(bagliTeklifOgesiId));
        }

        public void BagliTeklifOgesiEkle(BagliTeklifOgesi bagliTeklifOgesi)
        {
            if (bagliTeklifOgesi == null)
                throw new ArgumentNullException("bagliTeklifOgesi");

            _bagliTeklifOgesiDepo.Ekle(bagliTeklifOgesi);
            _önbellekYönetici.KalıpİleSil(BAGLITEKLIFOGESI_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(bagliTeklifOgesi);
        }

        public void BagliTeklifOgesiGüncelle(BagliTeklifOgesi bagliTeklifOgesi)
        {
            if (bagliTeklifOgesi == null)
                throw new ArgumentNullException("bagliTeklifOgesi");

            _bagliTeklifOgesiDepo.Güncelle(bagliTeklifOgesi);
            _önbellekYönetici.KalıpİleSil(BAGLITEKLIFOGESI_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(bagliTeklifOgesi);
        }

        public void BagliTeklifOgesiSil(BagliTeklifOgesi bagliTeklifOgesi)
        {
            if (bagliTeklifOgesi == null)
                throw new ArgumentNullException("bagliTeklifOgesi");

            _bagliTeklifOgesiDepo.Sil(bagliTeklifOgesi);
            _önbellekYönetici.KalıpİleSil(BAGLITEKLIFOGESI_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(bagliTeklifOgesi);
        }

        public IList<BagliTeklifOgesi> TümBagliTeklifOgesiAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(BAGLITEKLIFOGESI_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _bagliTeklifOgesiDepo.Tablo;
                return query.ToList();
            });
        }

        public IList<BagliTeklifOgesi> BagliTeklifOgesiAlTeklifId(int teklifId )
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
        public IList<BagliTeklifOgesi> BagliTeklifOgesiAlTeklifId(int teklifId, string durumu )
        {
            if (teklifId == 0)
                return null;
            if (durumu == "Hazırlık")
            {
                teklifId = _teklifServisi.TeklifAlId(teklifId).OrijinalTeklifId;
            }
            if (durumu == "Konfirme")
            {
                BagliTeklifOgesiAlTeklifId(teklifId);
            }
            if (durumu == "Tamamlandı")
            {
                BagliTeklifOgesiAlTeklifId(teklifId);
            }
            string key = string.Format(BAGLITEKLIFOGESI_BY_ID_KEY, teklifId);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _bagliTeklifOgesiDepo.Tablo.Where(x => x.TeklifId == teklifId).OrderBy(x => x.Vparent);
                return query.ToList();
            });
        }
    }

}
