using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Klasör;
using Core.Önbellek;
using Services.Olaylar;
using Core.Data;
using Core;

namespace Services.Klasör
{
    public partial class ÜlkeServisi : IÜlkeServisi
    {
        private const string ULKELER_ALL_KEY= "country.all-{0}-{1}";
        private const string ULKELER_PATTERN_KEY = "country.";

        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IDepo<Ülke> _ülkeDepo;
        private readonly ISiteContext _siteContext;
        //private readonly IKatalogAyarları _katalogAyarları;
        public ÜlkeServisi(IÖnbellekYönetici önbellekYönetici,
            IOlayYayınlayıcı olayYayınlayıcı,
            IDepo<Ülke> ülkeDepo,
            ISiteContext siteContext
            )
        {
            this._önbellekYönetici = önbellekYönetici;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._ülkeDepo = ülkeDepo;
            this._siteContext = siteContext;
        }
        public IList<Ülke> TümÜlkeleriAl(bool gizliOlanıGöster = false)
        {
            string key = string.Format(ULKELER_ALL_KEY, gizliOlanıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = _ülkeDepo.Tablo;
                if (!gizliOlanıGöster)
                    sorgu = sorgu.Where(c => c.Yayınlandı);
                sorgu = sorgu.OrderBy(c => c.Yayınlandı).ThenBy(c => c.Adı);

                if (!gizliOlanıGöster)
                {
                    var mevcutSiteId = _siteContext.MevcutSite.Id;
                    //yalnızca farklı varlıklar (grup id)
                    sorgu = from c in sorgu
                            group c by c.Id
                                into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    sorgu = sorgu.OrderBy(c => c.GörüntülenmeSırası).ThenBy(c => c.Adı);
                }

                var ülkeler = sorgu.ToList();
                return ülkeler;
            });
        }

        public Ülke ÜlkeAlId(int ülkeId)
        {
            if (ülkeId == 0)
                return null;
            return _ülkeDepo.AlId(ülkeId);
        }

        public IList<Ülke> ÜlkeAlIdler(int[] ülkeIdleri)
        {
            if (ülkeIdleri == null || ülkeIdleri.Length == 0)
                return new List<Ülke>();

            var sorgu = from c in _ülkeDepo.Tablo
                        where ülkeIdleri.Contains(c.Id)
                        select c;
            var ülkeler = sorgu.ToList();
            var sıralanmışÜlkeler = new List<Ülke>();
            foreach (int id in ülkeIdleri)
            {
                var ülke = ülkeler.Find(x => x.Id == id);
                if (ülke != null)
                    sıralanmışÜlkeler.Add(ülke);
            }
            return sıralanmışÜlkeler;
        }

        public Ülke ÜlkeAlÜçHarfIsoKodu(string üçHarfIsoKodu)
        {
            if (String.IsNullOrEmpty(üçHarfIsoKodu))
                return null;

            var sorgu = from c in _ülkeDepo.Tablo
                        where c.ÜçHarfIsoKodu == üçHarfIsoKodu
                        select c;
            var ülke = sorgu.FirstOrDefault();
            return ülke;
        }

        public Ülke ÜlkeAlİkiHarfIsoKodu(string ikiHarfIsoKodu)
        {
            if (String.IsNullOrEmpty(ikiHarfIsoKodu))
                return null;

            var sorgu = from c in _ülkeDepo.Tablo
                        where c.İkiHarfIsoKodu == ikiHarfIsoKodu
                        select c;
            var ülke = sorgu.FirstOrDefault();
            return ülke;
        }

        public void ÜlkeEkle(Ülke ülke)
        {
            if (ülke == null)
                throw new ArgumentNullException("ülke");

            _ülkeDepo.Ekle(ülke);
            _önbellekYönetici.KalıpİleSil(ULKELER_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(ülke);
        }

        public void ÜlkeGüncelle(Ülke ülke)
        {
            if (ülke == null)
                throw new ArgumentNullException("ülke");

            _ülkeDepo.Güncelle(ülke);
            _önbellekYönetici.KalıpİleSil(ULKELER_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(ülke);
        }

        public void ÜlkeSil(Ülke ülke)
        {
            if (ülke == null)
                throw new ArgumentNullException("ülke");

            _ülkeDepo.Sil(ülke);
            _önbellekYönetici.KalıpİleSil(ULKELER_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(ülke);
        }
    }
}
