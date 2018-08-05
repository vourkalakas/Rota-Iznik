using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Yapılandırma;
using Core.Domain.Yapılandırma;

namespace Services.Yapılandırma
{
    public partial interface IAyarlarServisi
    {
        Ayarlar AyarAlId(int ayarId);
        void AyarSil(Ayarlar Ayarlar);
        void AyarlarıSil(IList<Ayarlar> ayarlar);
        Ayarlar AyarAl(string key, int siteId = 0, bool paylaşılanDeğerYoksaYükle = false);
        T AyarAlKey<T>(string key, T defaultValue = default(T),
            int siteId = 0, bool paylaşılanDeğerYoksaYükle = false);
        void AyarAyarla<T>(string key, T value, int siteId = 0, bool önbelleğiTemizle = true);
        IList<Ayarlar> TümAyarlarıAl();
        bool AyarlarMevcut<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector, int siteId = 0)
            where T : IAyarlar, new();
        T AyarYükle<T>(int siteId = 0) where T : IAyarlar, new();
        void AyarKaydet<T>(T ayarlar, int siteId = 0) where T : IAyarlar, new();
        void AyarKaydet<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector,
            int siteId = 0, bool önbelleğiTemizle = true) where T : IAyarlar, new();
        void İptalEdilebilirAyarKaydet<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector,
            bool overrideForStore, int siteId = 0, bool önbelleğiTemizle = true) where T : IAyarlar, new();
        void AyarSil<T>() where T : IAyarlar, new();
        void AyarSil<T, TPropType>(T ayarlar,
            Expression<Func<T, TPropType>> keySelector, int siteId = 0) where T : IAyarlar, new();
        void ÖnbelleğiTemizle();
    }
}