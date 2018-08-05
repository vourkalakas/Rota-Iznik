using System;

namespace Core.Önbellek
{
    public interface IÖnbellekYönetici : IDisposable
    {
        T Al<T>(string key);
        void Ayarla(string key, object data, int önbellekZamanı);
        bool Ayarlandı(string key);
        void Sil(string key);
        void KalıpİleSil(string kalıp);
        void Temizle();
    }
}
