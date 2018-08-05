using System;
using System.Collections.Generic;
using Core.Domain.Mesajlar;
using Core.Data;
using Core.Domain.Katalog;
using Core.Önbellek;
using Services.Olaylar;
using System.Linq;

namespace Services.Mesajlar
{
    public partial class MesajTemasıServisi : IMesajTemasıServisi
    {
        private const string MESAJTEMASI_ALL_KEY = "mesajteması.tümü-{0}";
        private const string MESAJTEMASI_BY_NAME_KEY = "mesajteması.adı-{0}-{1}";
        private const string MESAJTEMASI_PATTERN_KEY = "mesajteması.";

        private readonly IDepo<MesajTeması> _mesajTemasıDepo;
        private readonly KatalogAyarları _katalogAyarları;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        public MesajTemasıServisi(IDepo<MesajTeması> mesajTemasıDepo,
                                 KatalogAyarları katalogAyarları,
                                  IÖnbellekYönetici önbellekYönetici,
                                 IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._mesajTemasıDepo = mesajTemasıDepo;
            this._katalogAyarları = katalogAyarları;
            this._önbellekYönetici = önbellekYönetici;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }
        public virtual MesajTeması MesajTemasıAlAdı(string mesajTemasıAdı, int siteId)
        {
            if (string.IsNullOrWhiteSpace(mesajTemasıAdı))
                throw new ArgumentNullException("mesajTemasıAdı");
            string key = string.Format(MESAJTEMASI_BY_NAME_KEY, mesajTemasıAdı, siteId);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = _mesajTemasıDepo.Tablo;
                sorgu = sorgu.Where(t => t.Adı == mesajTemasıAdı);
                sorgu = sorgu.OrderBy(t => t.Id);
                var temalar = sorgu.ToList();
                if(siteId>0)
                {
                    //siteye yetkilendir
                }
                return temalar.FirstOrDefault();
            });
        }

        public virtual MesajTeması MesajTemasıAlId(int mesajTemasıId)
        {
            if (mesajTemasıId == 0)
                return null;

            return _mesajTemasıDepo.AlId(mesajTemasıId);
        }

        public virtual void MesajTemasıEkle(MesajTeması mesajTeması)
        {
            if (mesajTeması == null)
                throw new ArgumentNullException("MesajTeması");
            _mesajTemasıDepo.Ekle(mesajTeması);
            _önbellekYönetici.KalıpİleSil(MESAJTEMASI_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(mesajTeması);
        }

        public virtual void MesajTemasıGüncelle(MesajTeması mesajTeması)
        {
            if (mesajTeması == null)
                throw new ArgumentNullException("MesajTeması");
            _mesajTemasıDepo.Güncelle(mesajTeması);
            _önbellekYönetici.KalıpİleSil(MESAJTEMASI_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(mesajTeması);
        }

        public virtual MesajTeması MesajTemasıKopyası(MesajTeması mesajTeması)
        {
            if (mesajTeması == null)
                throw new ArgumentNullException("mesajTeması");

            var mtKopya = new MesajTeması
            {
                Adı = mesajTeması.Adı,
                BccEmailAdresleri = mesajTeması.BccEmailAdresleri,
                Konu = mesajTeması.Konu,
                Gövde = mesajTeması.Gövde,
                Aktif = mesajTeması.Aktif,
                EkİndirmeId = mesajTeması.EkİndirmeId,
                EmailHesapId = mesajTeması.EmailHesapId,
                SitelerdeSınırlı = mesajTeması.SitelerdeSınırlı,
                GöndermedenÖnceGeciktir = mesajTeması.GöndermedenÖnceGeciktir,
                GecikmePeriodu = mesajTeması.GecikmePeriodu
            };

            MesajTemasıEkle(mtKopya);
            return mtKopya;
        }

        public virtual void MesajTemasıSil(MesajTeması mesajTeması)
        {
            if (mesajTeması == null)
                throw new ArgumentNullException("MesajTeması");
            _mesajTemasıDepo.Sil(mesajTeması);
            _önbellekYönetici.KalıpİleSil(MESAJTEMASI_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(mesajTeması);
        }

        public virtual IList<MesajTeması> TümMesajTeması(int siteId)
        {
            string key = string.Format(MESAJTEMASI_ALL_KEY, siteId);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = _mesajTemasıDepo.Tablo;
                sorgu = sorgu.OrderBy(t => t.Adı);
                if (siteId > 0 && !_katalogAyarları.IgnoreStoreLimitations)
                {
                    /*
                    sorgu = from t in sorgu
                            join sm in _storeMappingRepository.Table
                            on new { c1 = t.Id, c2 = "mesajTeması" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into t_sm
                            from sm in t_sm.DefaultIfEmpty()
                            where !t.LimitedToStores || storeId == sm.StoreId
                            select t;
                            */
                    sorgu = from t in sorgu
                            group t by t.Id
                            into tGroup
                            orderby tGroup.Key
                            select tGroup.FirstOrDefault();
                    sorgu = sorgu.OrderBy(t => t.Adı);
                }
                return sorgu.ToList();
            });
        }
    }
}
