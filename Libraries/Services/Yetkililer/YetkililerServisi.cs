using Core;
using Core.Data;
using Core.Domain.Yetkililer;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Yetkililer
{
    public class YetkiliServisi : IYetkiliServisi
    {
        private const string YETKİLİLER_ALL_KEY = "yetkililer.all-{0}-{1}";
        private const string YETKİLİLER_BY_ID_KEY = "yetkililer.id-{0}";
        private const string YETKİLİLER_PATTERN_KEY = "yetkililer.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Yetkili> _yetkililerDepo;
        public YetkiliServisi(IDepo<Yetkili> yetkililerDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._yetkililerDepo = yetkililerDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public IList<Yetkili> YetkiliAlGrup(int grup,int grupId)
        {
            if (grup == 0)
                return null;
            var query = _yetkililerDepo.Tablo.Where(x => x.Grup == grup && x.GrupId == grupId);
            return query.ToList();
        }
        public Yetkili YetkiliAlId(int yetkililerId)
        {
            if (yetkililerId == 0)
                return null;

            string key = string.Format(YETKİLİLER_BY_ID_KEY, yetkililerId);
            return _önbellekYönetici.Al(key, () => _yetkililerDepo.AlId(yetkililerId));
        }
        public void YetkiliEkle(Yetkili yetkililer)
        {
            if (yetkililer == null)
                throw new ArgumentNullException("yetkililer");

            _yetkililerDepo.Ekle(yetkililer);
            _önbellekYönetici.KalıpİleSil(YETKİLİLER_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(yetkililer);
        }

        public void YetkiliGüncelle(Yetkili yetkililer)
        {
            if (yetkililer == null)
                throw new ArgumentNullException("yetkililer");

            _yetkililerDepo.Güncelle(yetkililer);
            _önbellekYönetici.KalıpİleSil(YETKİLİLER_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(yetkililer);
        }

        public void YetkiliSil(Yetkili yetkililer)
        {
            if (yetkililer == null)
                throw new ArgumentNullException("yetkililer");

            _yetkililerDepo.Sil(yetkililer);
            _önbellekYönetici.KalıpİleSil(YETKİLİLER_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(yetkililer);
        }

        public virtual ISayfalıListe<Yetkili> TümYetkiliAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var sorgu = _yetkililerDepo.Tablo;
            sorgu = sorgu.OrderBy(pr => pr.OluşturulmaTarihi).ThenBy(pr => pr.Id);
            var tümYetkililer = new SayfalıListe<Yetkili>(sorgu, pageIndex, pageSize);
            return tümYetkililer;
        }
    }

}
