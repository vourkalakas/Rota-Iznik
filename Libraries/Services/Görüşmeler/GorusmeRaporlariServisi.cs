using Core;
using Core.Data;
using Core.Domain.Görüşmeler;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Görüşmeler
{
    public class GorusmeRaporlariServisi : IGorusmeRaporlariServisi
    {
        private const string GORUSMERAPORLARİ_ALL_KEY = "gorusmeRaporlari.all-{0}-{1}";
        private const string GORUSMERAPORLARİ_BY_ID_KEY = "gorusmeRaporlari.id-{0}";
        private const string GORUSMERAPORLARİ_PATTERN_KEY = "gorusmeRaporlari.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<GorusmeRaporlari> _gorusmeRaporlariDepo;
        public GorusmeRaporlariServisi(IDepo<GorusmeRaporlari> gorusmeRaporlariDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._gorusmeRaporlariDepo = gorusmeRaporlariDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public GorusmeRaporlari GorusmeRaporlariAlId(int gorusmeRaporlariId)
        {
            if (gorusmeRaporlariId == 0)
                return null;

            string key = string.Format(GORUSMERAPORLARİ_BY_ID_KEY, gorusmeRaporlariId);
            return _önbellekYönetici.Al(key, () => _gorusmeRaporlariDepo.AlId(gorusmeRaporlariId));
        }

        public void GorusmeRaporlariEkle(GorusmeRaporlari gorusmeRaporlari)
        {
            if (gorusmeRaporlari == null)
                throw new ArgumentNullException("gorusmeRaporlari");

            _gorusmeRaporlariDepo.Ekle(gorusmeRaporlari);
            _önbellekYönetici.KalıpİleSil(GORUSMERAPORLARİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(gorusmeRaporlari);
        }

        public void GorusmeRaporlariGüncelle(GorusmeRaporlari gorusmeRaporlari)
        {
            if (gorusmeRaporlari == null)
                throw new ArgumentNullException("gorusmeRaporlari");

            _gorusmeRaporlariDepo.Güncelle(gorusmeRaporlari);
            _önbellekYönetici.KalıpİleSil(GORUSMERAPORLARİ_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(gorusmeRaporlari);
        }

        public void GorusmeRaporlariSil(GorusmeRaporlari gorusmeRaporlari)
        {
            if (gorusmeRaporlari == null)
                throw new ArgumentNullException("gorusmeRaporlari");

            _gorusmeRaporlariDepo.Sil(gorusmeRaporlari);
            _önbellekYönetici.KalıpİleSil(GORUSMERAPORLARİ_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(gorusmeRaporlari);
        }

        public IList<GorusmeRaporlari> TümGorusmeRaporlariAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(GORUSMERAPORLARİ_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _gorusmeRaporlariDepo.Tablo;
                return query.ToList();
            });
        }
    }

}
