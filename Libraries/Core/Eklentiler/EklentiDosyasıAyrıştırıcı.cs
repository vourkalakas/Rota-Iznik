using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.Eklentiler
{
    public static class EklentiDosyasıAyrıştırıcı
    {
        public static IList<string> KuruluEklentiDosyalarınıAyrıştır(string dosyaYolu)
        {
            if (!File.Exists(dosyaYolu))
                return new List<string>();

            var text = File.ReadAllText(dosyaYolu);
            if (String.IsNullOrEmpty(text))
                return new List<string>();

            var lines = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(str))
                        continue;
                    lines.Add(str.Trim());
                }
            }
            return lines;
        }

        public static void KuruluEklentiDosyasınıKaydet(IList<String> EklentiSistemAdları, string dosyaYolu)
        {
            string sonuç = "";
            foreach (var sn in EklentiSistemAdları)
                sonuç += string.Format("{0}{1}", sn, Environment.NewLine);

            File.WriteAllText(dosyaYolu, sonuç);
        }

        public static EklentiTanımlayıcı EklentiAçıklamaDosyasıAyrıştırıcı(string dosyaYolu)
        {
            var tanımlayıcı = new EklentiTanımlayıcı();
            var text = File.ReadAllText(dosyaYolu);
            if (String.IsNullOrEmpty(text))
                return tanımlayıcı;

            var ayarlar = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(str))
                        continue;
                    ayarlar.Add(str.Trim());
                }
            }

            foreach (var ayar in ayarlar)
            {
                var ayırıcı = ayar.IndexOf(':');
                if (ayırıcı == -1)
                {
                    continue;
                }
                string key = ayar.Substring(0, ayırıcı).Trim();
                string value = ayar.Substring(ayırıcı + 1).Trim();

                switch (key)
                {
                    case "Grup":
                        tanımlayıcı.Grup = value;
                        break;
                    case "KısaAd":
                        tanımlayıcı.KısaAd = value;
                        break;
                    case "SistemAdı":
                        tanımlayıcı.SistemAdı = value;
                        break;
                    case "Sürüm":
                        tanımlayıcı.Sürüm = value;
                        break;
                    case "DesteklenenSürümler":
                        {
                            tanımlayıcı.DesteklenenSürümler = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .ToList();
                        }
                        break;
                    case "Yazar":
                        tanımlayıcı.Yazar = value;
                        break;
                    case "GörüntülemeSırası":
                        {
                            int görüntülemeSırası;
                            int.TryParse(value, out görüntülemeSırası);
                            tanımlayıcı.GörüntülemeSırası = görüntülemeSırası;
                        }
                        break;
                    case "EklentiDosyaAdı":
                        tanımlayıcı.EklentiDosyaAdı = value;
                        break;

                    case "KısıtlıMüsteriRolleriListesi":
                        {
                            foreach (var id in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
                            {
                                int roleId;
                                if (int.TryParse(id, out roleId))
                                    tanımlayıcı.KısıtlıMüsteriRolleriListesi.Add(roleId);
                            }
                        }
                        break;
                    case "Açıklama":
                        tanımlayıcı.Açıklama = value;
                        break;
                    default:
                        break;
                }
            }

            if (!tanımlayıcı.DesteklenenSürümler.Any())
                tanımlayıcı.DesteklenenSürümler.Add("2.00");

            return tanımlayıcı;
        }

        public static void EklentiAçıklamaDosyasınıKaydet(EklentiTanımlayıcı eklenti)
        {
            if (eklenti == null)
                throw new ArgumentException("eklenti");

            if (eklenti.OrijinalAssemblyDosyası == null)
                throw new Exception(string.Format("{0} eklentisi için orijinal assebmly dosyası yüklenemedi.", eklenti.SistemAdı));
            var dosyaYolu = Path.Combine(eklenti.OrijinalAssemblyDosyası.Directory.FullName, "Açıklama.txt");
            if (!File.Exists(dosyaYolu))
                throw new Exception(string.Format("{0} eklentisi için açıklama dosyası bulunamadı. {1}", eklenti.SistemAdı, dosyaYolu));

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("Grup", eklenti.Grup));
            keyValues.Add(new KeyValuePair<string, string>("KısaAd", eklenti.KısaAd));
            keyValues.Add(new KeyValuePair<string, string>("SistemAdı", eklenti.SistemAdı));
            keyValues.Add(new KeyValuePair<string, string>("Sürüm", eklenti.Sürüm));
            keyValues.Add(new KeyValuePair<string, string>("DesteklenenSürümler", string.Join(",", eklenti.DesteklenenSürümler)));
            keyValues.Add(new KeyValuePair<string, string>("Yazar", eklenti.Yazar));
            keyValues.Add(new KeyValuePair<string, string>("GörüntülemeSırası", eklenti.GörüntülemeSırası.ToString()));
            keyValues.Add(new KeyValuePair<string, string>("EklentiDosyaAdı", eklenti.EklentiDosyaAdı));
            keyValues.Add(new KeyValuePair<string, string>("Açıklama", eklenti.Açıklama));

            if (eklenti.KısıtlıMüsteriRolleriListesi.Any())
                keyValues.Add(new KeyValuePair<string, string>("KısıtlıMüsteriRolleriListesi", string.Join(",", eklenti.KısıtlıMüsteriRolleriListesi)));

            var sb = new StringBuilder();
            for (int i = 0; i < keyValues.Count; i++)
            {
                var key = keyValues[i].Key;
                var value = keyValues[i].Value;
                sb.AppendFormat("{0}: {1}", key, value);
                if (i != keyValues.Count - 1)
                    sb.Append(Environment.NewLine);
            }
            File.WriteAllText(dosyaYolu, sb.ToString());
        }
    }
}
