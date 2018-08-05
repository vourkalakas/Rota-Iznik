namespace Core.Önbellek
{
    public partial class NopNullCache : IStatikÖnbellekYönetici
    {
        public virtual T Al<T>(string key)
        {
            return default(T);
        }
        public virtual void Ayarla(string key, object data, int cacheTime)
        {
        }
        public bool Ayarlandı(string key)
        {
            return false;
        }
        public virtual void Sil(string key)
        {
        }
        public virtual void KalıpİleSil(string pattern)
        {
        }
        public virtual void Temizle()
        {
        }
        public virtual void Dispose()
        {
            //nothing special
        }
    }
}
