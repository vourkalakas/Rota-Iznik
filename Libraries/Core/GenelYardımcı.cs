using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace Core
{
    public partial class GenelYardımcı
    {
        internal static string KökKlasör { get; set; }
        public static string AboneMailAdresindenEminOl(string email)
        {
            string output = BoşKontrol(email);
            output = output.Trim();
            output = MaksimumUzunlukKontrol(output, 255);

            if (!GeçerliMail(output))
            {
                throw new Hata("Email geçerli değil.");
            }

            return output;
        }
        public static bool GeçerliMail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }
        public static bool GeçerliIpAdresi(string ipAdresi)
        {
            IPAddress ip;
            return IPAddress.TryParse(ipAdresi, out ip);
        }
        public static string RastgeleOndalıklıÜret(int uzunluk)
        {
            var rastgele = new Random();
            string str = string.Empty;
            for (int i = 0; i < uzunluk; i++)
                str = String.Concat(str, rastgele.Next(10).ToString());
            return str;
        }
        public static int RastgeleTamSayıÜret(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }
        public static string MaksimumUzunlukKontrol(string str, int maksUzunluk, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maksUzunluk)
            {
                var pLen = postfix == null ? 0 : postfix.Length;

                var sonuç = str.Substring(0, maksUzunluk - pLen);
                if (!String.IsNullOrEmpty(postfix))
                {
                    sonuç += postfix;
                }
                return sonuç;
            }

            return str;
        }
        public static string NümerikKontrol(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(p => char.IsDigit(p)).ToArray());
        }
        public static string BoşKontrol(string str)
        {
            return str ?? string.Empty;
        }
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            return stringsToValidate.Any(p => string.IsNullOrEmpty(p));
        }
        public static bool EşitSeriler<T>(T[] a1, T[] a2)
        {

            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var kıyasla = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!kıyasla.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
        
        public static void SeçenekAyarla(object model, string seçenekAdı, object değer)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (seçenekAdı == null) throw new ArgumentNullException("seçenekAdı");

            Type modelTipi = model.GetType();
            PropertyInfo pi = modelTipi.GetProperty(seçenekAdı);
            if (pi == null)
                throw new Hata("'{1}' model tipinde bir '{0}' seçenek adı bulunamadı.", seçenekAdı, modelTipi);
            if (!pi.CanWrite)
                throw new Hata("Seçenek adı '{0}' olan '{1}' model tipinin ayarlayıcısı yoktur.", seçenekAdı, modelTipi);
            if (değer != null && !değer.GetType().IsAssignableFrom(pi.PropertyType))
                değer = To(değer, pi.PropertyType);
            pi.SetValue(model, değer, new object[0]);
        }
        public static object To(object değer, Type HedefTip)
        {
            return To(değer, HedefTip, CultureInfo.InvariantCulture);
        }
        public static object To(object değer, Type HedefTip, CultureInfo culture)
        {
            if (değer != null)
            {
                var kaynakTipi = değer.GetType();

                var destinationConverter = TypeDescriptor.GetConverter(HedefTip);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(değer.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, değer);

                var KaynakDönüştürücü = TypeDescriptor.GetConverter(kaynakTipi);
                if (KaynakDönüştürücü != null && KaynakDönüştürücü.CanConvertTo(HedefTip))
                    return KaynakDönüştürücü.ConvertTo(null, culture, değer, HedefTip);

                if (HedefTip.IsEnum && değer is int)
                    return Enum.ToObject(HedefTip, (int)değer);

                if (!HedefTip.IsInstanceOfType(değer))
                    return Convert.ChangeType(değer, HedefTip, culture);
            }
            return değer;
        }
        public static T To<T>(object değer)
        {
            return (T)To(değer, typeof(T));
        }
        public static string EnumDönüştür(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            string sonuç = string.Empty;
            foreach (var c in str)
                if (c.ToString() != c.ToString().ToLower())
                    sonuç += " " + c.ToString();
                else
                    sonuç += c.ToString();

            //Boşluk olmamalıdır (örn. Ilk harf büyük harfli olduğunda)
            sonuç = sonuç.TrimStart();
            return sonuç;
        }
        public static void TelerikKültürAyarla()
        {
            var kültür = new CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentCulture = kültür;
            Thread.CurrentThread.CurrentUICulture = kültür;
        }
        public static int YıllarArasındakiFarkıAl(DateTime başlangıçTarihi, DateTime bitişTarihi)
        {

            int yıl = bitişTarihi.Year - başlangıçTarihi.Year;
            if (başlangıçTarihi > bitişTarihi.AddYears(-yıl))
                yıl--;
            return yıl;
        }
        internal static string BaseDirectory { get; set; }
        public static string MapPath(string yol)
        {
            yol = yol.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(BaseDirectory ?? string.Empty, yol);
        }
        public static object ÖzelAlanDeğeriAl(object hedef, string alanAdı)
        {
            if (hedef == null)
            {
                throw new ArgumentNullException("hedef", "Belirlenen hedef boş olamaz.");
            }

            if (string.IsNullOrEmpty(alanAdı))
            {
                throw new ArgumentException("alanAdı", "Belirlenen alan adı boş olamaz.");
            }

            var t = hedef.GetType();
            FieldInfo fi = null;

            while (t != null)
            {
                fi = t.GetField(alanAdı, BindingFlags.Instance | BindingFlags.NonPublic);

                if (fi != null) break;

                t = t.BaseType;
            }

            if (fi == null)
            {
                throw new Hata($"Alan adı '{alanAdı}' bulunamadı.");
            }

            return fi.GetValue(hedef);
        }
        public static void KlasörSil(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(path);

            foreach (var klasor in Directory.GetDirectories(path))
            {
                KlasörSil(klasor);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}

