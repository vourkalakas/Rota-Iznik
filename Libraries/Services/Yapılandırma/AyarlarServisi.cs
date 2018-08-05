using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core;
using Core.Önbellek;
using Core.Yapılandırma;
using Core.Data;
using Core.Domain.Yapılandırma;
using Services.Olaylar;

namespace Services.Yapılandırma
{
    public partial class AyarlarServisi : IAyarlarServisi
    {
        #region Constants
        private const string AYARLAR_ALL_KEY = "TS.ayarlar.all";
        private const string AYARLAR_PATTERN_KEY = "TS.ayarlar.";

        #endregion

        #region Fields

        private readonly IDepo<Ayarlar> _ayarlarDepo;
        private readonly IOlayYayınlayıcı _olayYayınlayısı;
        private readonly IÖnbellekYönetici _önbellekYönetici;

        #endregion

        #region Ctor
        public AyarlarServisi(IÖnbellekYönetici önbellekYönetici, IOlayYayınlayıcı eventPublisher,
            IDepo<Ayarlar> settingRepository)
        {
            this._önbellekYönetici = önbellekYönetici;
            this._olayYayınlayısı = eventPublisher;
            this._ayarlarDepo = settingRepository;
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class ÖnbellekİçinAyarlar
        {
            public int Id { get; set; }
            public string Adı { get; set; }
            public string Değer { get; set; }
            public int SiteId { get; set; }
        }

        #endregion

        #region Utilities
        protected virtual IDictionary<string, IList<ÖnbellekİçinAyarlar>> ÖnbellektekiTümAyarlarıAl()
        {
            //önbellek
            string key = string.Format(AYARLAR_ALL_KEY);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = from s in _ayarlarDepo.Tabloİzlemesiz
                            orderby s.Ad, s.SiteId
                            select s;
                var ayarlar = sorgu.ToList();
                var dictionary = new Dictionary<string, IList<ÖnbellekİçinAyarlar>>();
                foreach (var s in ayarlar)
                {
                    var kaynakAdı = s.Ad.ToLowerInvariant();
                    var önbellekİçinAyarlar = new ÖnbellekİçinAyarlar
                    {
                        Id = s.Id,
                        Adı = s.Ad,
                        Değer = s.Değer,
                        SiteId = s.SiteId
                    };
                    if (!dictionary.ContainsKey(kaynakAdı))
                    {
                        //ilk ayar
                        dictionary.Add(kaynakAdı, new List<ÖnbellekİçinAyarlar>
                        {
                            önbellekİçinAyarlar
                        });
                    }
                    else
                    {
                        //zaten eklendi
                        dictionary[kaynakAdı].Add(önbellekİçinAyarlar);
                    }
                }
                return dictionary;
            });
        }

        #endregion

        #region Methods
        public virtual void AyarEkle(Ayarlar ayar, bool önbelleğiTemizle = true)
        {
            if (ayar == null)
                throw new ArgumentNullException("ayar");

            _ayarlarDepo.Ekle(ayar);

            //önbellek
            if (önbelleğiTemizle)
                _önbellekYönetici.KalıpİleSil(AYARLAR_PATTERN_KEY);

            //olay bildirimi
            _olayYayınlayısı.OlayEklendi(ayar);
        }
        public virtual void AyarGüncelle(Ayarlar ayar, bool önbelleğiTemizle = true)
        {
            if (ayar == null)
                throw new ArgumentNullException("ayar");

            _ayarlarDepo.Güncelle(ayar);

            //önbellek
            if (önbelleğiTemizle)
                _önbellekYönetici.KalıpİleSil(AYARLAR_PATTERN_KEY);

            //olay bildirimi
            _olayYayınlayısı.OlayGüncellendi(ayar);
        }
        public virtual void AyarSil(Ayarlar ayar)
        {
            if (ayar == null)
                throw new ArgumentNullException("ayar");

            _ayarlarDepo.Sil(ayar);

            //önbellek
            _önbellekYönetici.KalıpİleSil(AYARLAR_PATTERN_KEY);

            //olay bildirimi
            _olayYayınlayısı.OlaySilindi(ayar);
        }
        public virtual void AyarlarıSil(IList<Ayarlar> ayarlar)
        {
            if (ayarlar == null)
                throw new ArgumentNullException("ayarlar");

            _ayarlarDepo.Sil(ayarlar);

            //önbellek
            _önbellekYönetici.KalıpİleSil(AYARLAR_PATTERN_KEY);

            //olay bildirimi
            foreach (var ayar in ayarlar)
            {
                _olayYayınlayısı.OlaySilindi(ayar);
            }
        }
        public virtual Ayarlar AyarAlId(int ayarId)
        {
            if (ayarId == 0)
                return null;

            return _ayarlarDepo.AlId(ayarId);
        }
        public virtual Ayarlar AyarAl(string key, int siteId = 0, bool paylaşılanDeğerYoksaYükle = false)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            var ayarlar = ÖnbellektekiTümAyarlarıAl();
            key = key.Trim().ToLowerInvariant();
            if (ayarlar.ContainsKey(key))
            {
                var settingsByKey = ayarlar[key];
                var ayar = settingsByKey.FirstOrDefault(x => x.SiteId == siteId);

                //paylaşılan değeri yükle?
                if (ayar == null && siteId > 0 && paylaşılanDeğerYoksaYükle)
                    ayar = settingsByKey.FirstOrDefault(x => x.SiteId == 0);

                if (ayar != null)
                    return AyarAlId(ayar.Id);
            }

            return null;
        }
        public virtual T AyarAlKey<T>(string key, T varsayılanDeğer = default(T),
            int SiteId = 0, bool paylaşılanDeğerYoksaYükle = false)
        {
            if (String.IsNullOrEmpty(key))
                return varsayılanDeğer;

            var ayarlar = ÖnbellektekiTümAyarlarıAl();
            key = key.Trim().ToLowerInvariant();
            if (ayarlar.ContainsKey(key))
            {
                var settingsByKey = ayarlar[key];
                var ayar = settingsByKey.FirstOrDefault(x => x.SiteId == SiteId);

                //paylaşılan değeri yükle?
                if (ayar == null && SiteId > 0 && paylaşılanDeğerYoksaYükle)
                    ayar = settingsByKey.FirstOrDefault(x => x.SiteId == 0);

                if (ayar != null)
                    return GenelYardımcı.To<T>(ayar.Değer);
            }

            return varsayılanDeğer;
        }
        public virtual void AyarAyarla<T>(string key, T değer, int SiteId = 0, bool önbelleğiTemizle = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            string valueStr = TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(değer);

            var tümAyarlar = ÖnbellektekiTümAyarlarıAl();
            var önbellekİçinAyarlar = tümAyarlar.ContainsKey(key) ?
                tümAyarlar[key].FirstOrDefault(x => x.SiteId == SiteId) : null;
            if (önbellekİçinAyarlar != null)
            {
                //güncelle
                var ayar = AyarAlId(önbellekİçinAyarlar.Id);
                ayar.Değer = valueStr;
                AyarGüncelle(ayar, önbelleğiTemizle);
            }
            else
            {
                //ekle
                var ayar = new Ayarlar
                {
                    Ad = key,
                    Değer = valueStr,
                    SiteId = SiteId
                };
                AyarEkle(ayar, önbelleğiTemizle);
            }
        }
        public virtual IList<Ayarlar> TümAyarlarıAl()
        {
            var sorgu = from s in _ayarlarDepo.Tablo
                        orderby s.Ad, s.SiteId
                        select s;
            var ayarlar = sorgu.ToList();
            return ayarlar;
        }
        public virtual bool AyarlarMevcut<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector, int SiteId = 0)
            where T : IAyarlar, new()
        {
            string key = ayarlar.GetSettingKey(keySelector);

            var ayar = AyarAlKey<string>(key, SiteId: SiteId);
            return ayar != null;
        }
        public virtual T AyarYükle<T>(int SiteId = 0) where T : IAyarlar, new()
        {
            var ayarlar = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                //okuyup yazılabilen seçenekleri al
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //siteden yükle
                var ayar = AyarAlKey<string>(key, SiteId: SiteId, paylaşılanDeğerYoksaYükle: true);
                if (ayar == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(ayar))
                    continue;

                object value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(ayar);

                //seçenek ayarla
                prop.SetValue(ayarlar, value, null);
            }

            return ayarlar;
        }
        public virtual void AyarKaydet<T>(T ayarlar, int SiteId = 0) where T : IAyarlar, new()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                //okuyup yazılabilen seçenekleri al
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                dynamic value = prop.GetValue(ayarlar, null);
                if (value != null)
                    AyarAyarla(key, value, SiteId, false);
                else
                    AyarAyarla(key, "", SiteId, false);
            }

            ÖnbelleğiTemizle();
        }
        public virtual void AyarKaydet<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector,
            int SiteId = 0, bool önbelleğiTemizle = true) where T : IAyarlar, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = ayarlar.GetSettingKey(keySelector);
            //Duck typing is not supported in C#. That's why we're using dynamic type
            dynamic value = propInfo.GetValue(ayarlar, null);
            if (value != null)
                AyarAyarla(key, value, SiteId, önbelleğiTemizle);
            else
                AyarAyarla(key, "", SiteId, önbelleğiTemizle);
        }
        public virtual void İptalEdilebilirAyarKaydet<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector,
            bool overrideForStore, int SiteId = 0, bool önbelleğiTemizle = true) where T : IAyarlar, new()
        {
            if (overrideForStore || SiteId == 0)
                AyarKaydet(ayarlar, keySelector, SiteId, önbelleğiTemizle);
            else if (SiteId > 0)
                AyarSil(ayarlar, keySelector, SiteId);
        }
        public virtual void AyarSil<T>() where T : IAyarlar, new()
        {
            var silinecekAyarlar = new List<Ayarlar>();
            var tümAyarlar = TümAyarlarıAl();
            foreach (var prop in typeof(T).GetProperties())
            {
                string key = typeof(T).Name + "." + prop.Name;
                silinecekAyarlar.AddRange(tümAyarlar.Where(x => x.Ad.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            AyarlarıSil(silinecekAyarlar);
        }
        public virtual void AyarSil<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector, int SiteId = 0) where T : IAyarlar, new()
        {
            string key = ayarlar.GetSettingKey(keySelector);
            key = key.Trim().ToLowerInvariant();

            var tümAyarlar = ÖnbellektekiTümAyarlarıAl();
            var önbellekİçinAyarlar = tümAyarlar.ContainsKey(key) ?
                tümAyarlar[key].FirstOrDefault(x => x.SiteId == SiteId) : null;
            if (önbellekİçinAyarlar != null)
            {
                //güncelle
                var ayar = AyarAlId(önbellekİçinAyarlar.Id);
                AyarSil(ayar);
            }
        }
        public virtual void ÖnbelleğiTemizle()
        {
            _önbellekYönetici.KalıpİleSil(AYARLAR_PATTERN_KEY);
        }

        #endregion
    }
}
