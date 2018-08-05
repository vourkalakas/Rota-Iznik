using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Core.Önbellek
{
    public partial class İstekBaşınaÖnbellekYöneticisi : IÖnbellekYönetici
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public İstekBaşınaÖnbellekYöneticisi(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        protected virtual IDictionary<object, object> GetItems()
        {
            return _httpContextAccessor.HttpContext?.Items;
        }
        public virtual T Al<T>(string key)
        {
            var items = GetItems();
            if (items == null)
                return default(T);

            return (T)items[key];
        }
        public virtual void Ayarla(string key, object data, int cacheTime)
        {
            var items = GetItems();
            if (items == null)
                return;

            if (data != null)
                items[key] = data;
        }
        public virtual bool Ayarlandı(string key)
        {
            var items = GetItems();
            return items?[key] != null;
        }
        public virtual void Sil(string key)
        {
            var items = GetItems();
            items?.Remove(key);
        }
        public virtual void KalıpİleSil(string pattern)
        {
            var items = GetItems();
            if (items == null)
                return;

            this.KalıpİleSil(pattern, items.Keys.Select(p => p.ToString()));
        }
        public virtual void Temizle()
        {
            var items = GetItems();
            items?.Clear();
        }
        public virtual void Dispose()
        {
        }
    }
}
