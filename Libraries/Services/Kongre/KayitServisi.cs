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
    public class KayitServisi : IKayitServisi
    {
        private const string KAYİT_ALL_KEY = "kayit.all-{0}-{1}";
        private const string KAYİT_BY_ID_KEY = "kayit.id-{0}";
        private const string KAYİT_PATTERN_KEY = "kayit.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Kayit> _kayitDepo;
        public KayitServisi(IDepo<Kayit> kayitDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._kayitDepo = kayitDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Kayit KayitAlId(int kayitId)
        {
            if (kayitId == 0)
                return null;

            string key = string.Format(KAYİT_BY_ID_KEY, kayitId);
            return _önbellekYönetici.Al(key, () => _kayitDepo.AlId(kayitId));
        }

        public void KayitEkle(Kayit kayit)
        {
            if (kayit == null)
                throw new ArgumentNullException("kayit");

            _kayitDepo.Ekle(kayit);
            _önbellekYönetici.KalıpİleSil(KAYİT_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(kayit);
        }

        public void KayitGüncelle(Kayit kayit)
        {
            if (kayit == null)
                throw new ArgumentNullException("kayit");

            _kayitDepo.Güncelle(kayit);
            _önbellekYönetici.KalıpİleSil(KAYİT_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(kayit);
        }

        public void KayitSil(Kayit kayit)
        {
            if (kayit == null)
                throw new ArgumentNullException("kayit");

            _kayitDepo.Sil(kayit);
            _önbellekYönetici.KalıpİleSil(KAYİT_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(kayit);
        }

        public IList<Kayit> TümKayitAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            
                var query = _kayitDepo.Tablo;
                return query.ToList();
           
        }
        public IList<Kayit> KayitAlKongreId(int kongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(KAYİT_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _kayitDepo.Tablo.Where(x=>x.KongreId==kongreId);
                return query.ToList();
            });
        }
    }

}
