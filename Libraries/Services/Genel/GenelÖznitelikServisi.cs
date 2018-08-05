using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Önbellek;
using Core.Data;
using Core.Domain.Genel;
using Data;
using Services.Olaylar;

namespace Services.Genel
{
    public partial class GenelÖznitelikServisi : IGenelÖznitelikServisi
    {
        #region Constants

        private const string GENELOZNITELIK_KEY = "TS.genelöznitelik.{0}-{1}";
        private const string GENELOZNITELIK_PATTERN_KEY = "TS.genelöznitelik.";
        #endregion

        #region Fields

        private readonly IDepo<GenelÖznitelik> _genelÖznitelikDeposu;
        private readonly IÖnbellekYönetici _önbelekYönetici;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;

        #endregion

        #region Ctor
        public GenelÖznitelikServisi(IÖnbellekYönetici önbellekYönetici,
            IDepo<GenelÖznitelik> genelÖznitelikDeposu,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._önbelekYönetici = önbellekYönetici;
            this._genelÖznitelikDeposu = genelÖznitelikDeposu;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }

        #endregion

        #region Methods
        public virtual void ÖznitelikSil(GenelÖznitelik öznitelik)
        {
            if (öznitelik == null)
                throw new ArgumentNullException("öznitelik");

            _genelÖznitelikDeposu.Sil(öznitelik);

            //önbellek
            _önbelekYönetici.KalıpİleSil(GENELOZNITELIK_PATTERN_KEY);

            //olay bildirimleri
            _olayYayınlayıcı.OlaySilindi(öznitelik);
        }
        public virtual void ÖznitelikleriSil(IList<GenelÖznitelik> öznitelikler)
        {
            if (öznitelikler == null)
                throw new ArgumentNullException("öznitelikler");

            _genelÖznitelikDeposu.Sil(öznitelikler);

            //önbellek
            _önbelekYönetici.KalıpİleSil(GENELOZNITELIK_PATTERN_KEY);

            //olay bildirimleri
            foreach (var öznitelik in öznitelikler)
            {
                _olayYayınlayıcı.OlaySilindi(öznitelik);
            }
        }
        public virtual GenelÖznitelik ÖznitelikAlId(int öznitelikId)
        {
            if (öznitelikId == 0)
                return null;

            return _genelÖznitelikDeposu.AlId(öznitelikId);
        }
        public virtual void ÖznitelikEkle(GenelÖznitelik öznitelik)
        {
            if (öznitelik == null)
                throw new ArgumentNullException("öznitelik");

            _genelÖznitelikDeposu.Ekle(öznitelik);

            //önbellek
            _önbelekYönetici.KalıpİleSil(GENELOZNITELIK_PATTERN_KEY);

            //olay bildirimleri
            _olayYayınlayıcı.OlayEklendi(öznitelik);
        }
        public virtual void ÖznitelikGüncelle(GenelÖznitelik öznitelik)
        {
            if (öznitelik == null)
                throw new ArgumentNullException("öznitelik");

            _genelÖznitelikDeposu.Güncelle(öznitelik);

            //önbellek
            _önbelekYönetici.KalıpİleSil(GENELOZNITELIK_PATTERN_KEY);

            //olay bildirimleri
            _olayYayınlayıcı.OlayGüncellendi(öznitelik);
        }
        public virtual IList<GenelÖznitelik> VarlıkİçinÖznitelikleriAl(int varlıkId, string keyGroup)
        {
            string key = string.Format(GENELOZNITELIK_KEY, varlıkId, keyGroup);
            return _önbelekYönetici.Al(key, () =>
            {
                var sorgu = from ga in _genelÖznitelikDeposu.Tablo
                            where ga.VarlıkId == varlıkId &&
                            ga.KeyGroup == keyGroup
                            select ga;
                var öznitelikler = sorgu.ToList();
                return öznitelikler;
            });
        }
        public virtual void ÖznitelikKaydet<TPropType>(TemelVarlık varlık, string key, TPropType değer, int siteId = 0)
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            if (key == null)
                throw new ArgumentNullException("key");

            string keyGroup = varlık.GetUnproxiedEntityType().Name;

            var props = VarlıkİçinÖznitelikleriAl(varlık.Id, keyGroup)
                .Where(x => x.SiteId == siteId)
                .ToList();
            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); 

            var değerStr = GenelYardımcı.To<string>(değer);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(değerStr))
                {
                    //sil
                    ÖznitelikSil(prop);
                }
                else
                {
                    //güncelle
                    prop.Value = değerStr;
                    ÖznitelikGüncelle(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(değerStr))
                {
                    //insert
                    prop = new GenelÖznitelik
                    {
                        VarlıkId = varlık.Id,
                        Key = key,
                        KeyGroup = keyGroup,
                        Value = değerStr,
                        SiteId = siteId,

                    };
                    ÖznitelikEkle(prop);
                }
            }
        }

        #endregion
    }
}
