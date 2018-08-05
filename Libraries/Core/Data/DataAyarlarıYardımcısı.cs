using System;

namespace Core.Data
{
    public partial class DataAyarlarıYardımcısı
    {
        private static bool? _databaseYüklendi;
        public static bool DatabaseYüklendi()
        {
            if (!_databaseYüklendi.HasValue)
            {
                var yönetici = new DataAyarlarıYönetici();
                var ayarlar = yönetici.AyarlarıYükle();
                _databaseYüklendi = ayarlar != null && !String.IsNullOrEmpty(ayarlar.DataConnectionString);
            }
            return _databaseYüklendi.Value;
        }
        public static void ÖnbelleğiTemizle()
        {
            _databaseYüklendi = null;
        }
    }
}
