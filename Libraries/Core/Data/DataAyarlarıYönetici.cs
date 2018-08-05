using Core.Altyapı;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Core.Data
{
    public partial class DataAyarlarıYönetici
    {
        private const string EskiDataAyarlarıDosyaYolu = "~/App_Data/Ayarlar.txt";
        private const string DataAyarlarıDosyaYolu_ = "~/App_Data/dataAyarları.json";
        public static string DataAyarlarıDosyaYolu => DataAyarlarıDosyaYolu_;
        public virtual DataAyarları AyarlarıYükle(string dosyaYolu = null, bool ayarlarıYenidenYükle = false)
        {
            if (!ayarlarıYenidenYükle && Singleton<DataAyarları>.Instance != null)
                return Singleton<DataAyarları>.Instance;

            dosyaYolu = dosyaYolu ?? GenelYardımcı.MapPath(DataAyarlarıDosyaYolu);
            if (!File.Exists(dosyaYolu))
            {
                dosyaYolu = GenelYardımcı.MapPath(EskiDataAyarlarıDosyaYolu);
                if (!File.Exists(dosyaYolu))
                    return new DataAyarları();

                //get data settings from the old txt file
                var DataAyarları = new DataAyarları();
                using (var reader = new StringReader(File.ReadAllText(dosyaYolu)))
                {
                    var settingsLine = string.Empty;
                    while ((settingsLine = reader.ReadLine()) != null)
                    {
                        var separatorIndex = settingsLine.IndexOf(':');
                        if (separatorIndex == -1)
                            continue;

                        var key = settingsLine.Substring(0, separatorIndex).Trim();
                        var value = settingsLine.Substring(separatorIndex + 1).Trim();

                        switch (key)
                        {
                            case "DataProvider":
                                DataAyarları.DataSağlayıcı = value;
                                continue;
                            case "DataConnectionString":
                                DataAyarları.DataConnectionString = value;
                                continue;
                            default:
                                DataAyarları.HamDataAyarları.Add(key, value);
                                continue;
                        }
                    }
                }
                AyarlarıKaydet(DataAyarları);
                File.Delete(dosyaYolu);
                Singleton<DataAyarları>.Instance = DataAyarları;
                return Singleton<DataAyarları>.Instance;
            }
            var text = File.ReadAllText(dosyaYolu);
            if (string.IsNullOrEmpty(text))
                return new DataAyarları();
            Singleton<DataAyarları>.Instance = JsonConvert.DeserializeObject<DataAyarları>(text);
            return Singleton<DataAyarları>.Instance;
        }
        public virtual void AyarlarıKaydet(DataAyarları settings)
        {
            Singleton<DataAyarları>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));
            var filePath = GenelYardımcı.MapPath(DataAyarlarıDosyaYolu);
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
            }
            var text = JsonConvert.SerializeObject(Singleton<DataAyarları>.Instance, Formatting.Indented);
            File.WriteAllText(filePath, text);
        }
    }
}
