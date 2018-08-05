using Core;
using Core.Data;
using Core.Domain.Notlar;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Notlar
{
    public class NotServisi : INotServisi
    {
        private const string NOT_ALL_KEY = "not.all-{0}-{1}";
        private const string NOT_BY_ID_KEY = "not.id-{0}";
        private const string NOT_PATTERN_KEY = "not.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Not> _notDepo;
        public NotServisi(IDepo<Not> notDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._notDepo = notDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Not NotAlId(int notId)
        {
            if (notId == 0)
                return null;

            string key = string.Format(NOT_BY_ID_KEY, notId);
            return _önbellekYönetici.Al(key, () => _notDepo.AlId(notId));
        }

        public void NotEkle(Not not)
        {
            if (not == null)
                throw new ArgumentNullException("not");

            _notDepo.Ekle(not);
            _önbellekYönetici.KalıpİleSil(NOT_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(not);
        }

        public void NotGüncelle(Not not)
        {
            if (not == null)
                throw new ArgumentNullException("not");

            _notDepo.Güncelle(not);
            _önbellekYönetici.KalıpİleSil(NOT_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(not);
        }

        public void NotSil(Not not)
        {
            if (not == null)
                throw new ArgumentNullException("not");

            _notDepo.Sil(not);
            _önbellekYönetici.KalıpİleSil(NOT_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(not);
        }

        public IList<Not> TümNotAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(NOT_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _notDepo.Tablo;
                return query.ToList();
            });
        }
        public IList<Not> NotAlId(int userId, string grup, int? grupId)
        {
            var query = _notDepo.Tablo;
            query = query.Where(x => x.KullanıcıId == userId);
            if (!string.IsNullOrEmpty(grup))
            {
                query = query.Where(x => x.Grup == grup);
            }
            if (grupId.HasValue)
            {
                query = query.Where(x => x.GrupId == grupId);
            }
            if (query.Count() > 0)
            {
                query = query.OrderByDescending(x => x.Id);
            }
            return query.ToList();
        }
    }

}
