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
    public class BankalarServisi : IBankalarServisi
    {
        private const string BANKALAR_ALL_KEY = "bankalar.all-{0}-{1}";
        private const string BANKALAR_BY_ID_KEY = "bankalar.id-{0}";
        private const string BANKALAR_PATTERN_KEY = "bankalar.";
        private readonly IDepo<Banka> _bankaDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public BankalarServisi(IDepo<Banka> bankaDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._bankaDepo = bankaDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Banka BankaAlId(int bankaId)
        {
            if (bankaId == 0)
                return null;

            string key = string.Format(BANKALAR_BY_ID_KEY, bankaId);
            return _önbellekYönetici.Al(key, () => _bankaDepo.AlId(bankaId));
        }

        public void BankaEkle(Banka banka)
        {
            if (banka == null)
                throw new ArgumentNullException("banka");

            _bankaDepo.Ekle(banka);
            _önbellekYönetici.KalıpİleSil(BANKALAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(banka);
        }

        public void BankaGüncelle(Banka banka)
        {
            if (banka == null)
                throw new ArgumentNullException("banka");

            _bankaDepo.Güncelle(banka);
            _önbellekYönetici.KalıpİleSil(BANKALAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(banka);
        }

        public void BankaSil(Banka banka)
        {
            if (banka == null)
                throw new ArgumentNullException("banka");

            _bankaDepo.Sil(banka);
            _önbellekYönetici.KalıpİleSil(BANKALAR_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(banka);
        }

        public IList<Banka> TümBankalarıAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(BANKALAR_ALL_KEY,  AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _bankaDepo.Tablo;
                /*query = query.OrderBy(t => t.GörüntülenmeSırası).ThenBy(t => t.SistemAdı);
                
                if (!gizliOlanlarıGöster)
                    query = query.Where(t => t.Yayınlandı);*/
                return query.ToList();
            });
        }
    }
}
