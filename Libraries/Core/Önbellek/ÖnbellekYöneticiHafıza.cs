using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Core.Önbellek
{
    public partial class ÖnbellekYöneticiHafıza : IStatikÖnbellekYönetici
    {
        #region Fields

        private readonly IMemoryCache _önbellek;
        protected CancellationTokenSource _iptalAnahtarKaynağı;
        protected static readonly ConcurrentDictionary<string, bool> _tümAnahtarlar;

        #endregion

        #region Ctor
        static ÖnbellekYöneticiHafıza()
        {
            _tümAnahtarlar = new ConcurrentDictionary<string, bool>();
        }
        
        public ÖnbellekYöneticiHafıza(IMemoryCache önbellek)
        {
            _önbellek = önbellek;
            _iptalAnahtarKaynağı = new CancellationTokenSource();
        }

        #endregion

        #region Utilities
        
        protected MemoryCacheEntryOptions ÖnbellekHafızaSeçenekleriAl(int cacheTime)
        {
            var seçenekler = new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(_iptalAnahtarKaynağı.Token))
                .RegisterPostEvictionCallback(PostEviction);

            seçenekler.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime);

            return seçenekler;
        }
        protected string AnahtarEkle(string anahtar)
        {
            _tümAnahtarlar.TryAdd(anahtar, true);
            return anahtar;
        }
        protected string AnahtarSil(string anahtar)
        {
            AnahtarSilmeyiDene(anahtar);
            return anahtar;
        }
        protected void AnahtarSilmeyiDene(string anahtar)
        {
            if (!_tümAnahtarlar.TryRemove(anahtar, out bool _))
                _tümAnahtarlar.TryUpdate(anahtar, false, false);
        }
        private void AnahtarlarıTemizle()
        {
            foreach (var anahtar in _tümAnahtarlar.Where(p => !p.Value).Select(p => p.Key).ToList())
            {
                AnahtarSil(anahtar);
            }
        }
        private void PostEviction(object anahtar, object value, EvictionReason sonuç, object durum)
        {
            if (sonuç == EvictionReason.Replaced)
                return;
            AnahtarlarıTemizle();
            AnahtarSilmeyiDene(anahtar.ToString());
        }

        #endregion

        #region Methods
        
        public virtual T Al<T>(string anahtar)
        {
            return _önbellek.Get<T>(anahtar);
        }
        public virtual void Ayarla(string anahtar, object data, int önbellekZamanı)
        {
            if (data != null)
            {
                _önbellek.Set(AnahtarEkle(anahtar), data, ÖnbellekHafızaSeçenekleriAl(önbellekZamanı));
            }
        }
        public virtual bool Ayarlandı(string anahtar)
        {
            return _önbellek.TryGetValue(anahtar, out object _);
        }
        public virtual void Sil(string anahtar)
        {
            _önbellek.Remove(AnahtarSil(anahtar));
        }
        public virtual void KalıpİleSil(string pattern)
        {
            this.KalıpİleSil(pattern, _tümAnahtarlar.Where(p => p.Value).Select(p => p.Key));
        }
        public virtual void Temizle()
        {
            _iptalAnahtarKaynağı.Cancel();
            _iptalAnahtarKaynağı.Dispose();
            _iptalAnahtarKaynağı = new CancellationTokenSource();
        }
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
