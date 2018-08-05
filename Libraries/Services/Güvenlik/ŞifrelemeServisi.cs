using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Core.Domain.Güvenlik;

namespace Services.Güvenlik
{
    public class ŞifrelemeServisi : IŞifrelemeServisi
    {
        private readonly GüvenlikAyarları _güvenlikAyarları;
        public ŞifrelemeServisi(GüvenlikAyarları güvenlikAyarları)
        {
            this._güvenlikAyarları = güvenlikAyarları;
        }
        public virtual string SaltAnahtarıOluştur(int size)
        {
            //cryptographic rastgele sayı üret
            using (var sağlayıcı = new RNGCryptoServiceProvider())
            {
                var buff = new byte[size];
                sağlayıcı.GetBytes(buff);

                // Rastgele sayının Base64 dize gösterimini döndürür
                return Convert.ToBase64String(buff);
            }
        }
        public virtual string ŞifreHashOluştur(string şifre, string saltanahtarı, string şifreFormatı = "SHA1")
        {
            return HashOluştur(Encoding.UTF8.GetBytes(String.Concat(şifre, saltanahtarı)), şifreFormatı);
        }
        public virtual string HashOluştur(byte[] data, string hashAlgorithm = "SHA1")
        {
            if (String.IsNullOrEmpty(hashAlgorithm))
                hashAlgorithm = "SHA1";

            var algorithm = HashAlgorithm.Create(hashAlgorithm);
            if (algorithm == null)
                throw new ArgumentException("Tanınmayan karma adı");

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }
        public virtual string TextŞifrele(string düzMetin, string şifrelemeÖzelAnahtarı = "")
        {
            if (string.IsNullOrEmpty(düzMetin))
                return düzMetin;

            if (String.IsNullOrEmpty(şifrelemeÖzelAnahtarı))
                şifrelemeÖzelAnahtarı = _güvenlikAyarları.ŞifrelemeAnahtarı;

            using (var sağlayıcı = new TripleDESCryptoServiceProvider())
            {
                sağlayıcı.Key = Encoding.ASCII.GetBytes(şifrelemeÖzelAnahtarı.Substring(0, 16));
                sağlayıcı.IV = Encoding.ASCII.GetBytes(şifrelemeÖzelAnahtarı.Substring(8, 8));

                byte[] şifrelenmişBinary = MetniBelleğeŞifrele(düzMetin, sağlayıcı.Key, sağlayıcı.IV);
                return Convert.ToBase64String(şifrelenmişBinary);
            }
        }
        public virtual string TextÇöz(string şifreleyiciText, string şifrelemeÖzelAnahtarı = "")
        {
            if (String.IsNullOrEmpty(şifreleyiciText))
                return şifreleyiciText;

            if (String.IsNullOrEmpty(şifrelemeÖzelAnahtarı))
                şifrelemeÖzelAnahtarı = _güvenlikAyarları.ŞifrelemeAnahtarı;

            using (var sağlayıcı = new TripleDESCryptoServiceProvider())
            {
                sağlayıcı.Key = Encoding.ASCII.GetBytes(şifrelemeÖzelAnahtarı.Substring(0, 16));
                sağlayıcı.IV = Encoding.ASCII.GetBytes(şifrelemeÖzelAnahtarı.Substring(8, 8));

                byte[] buffer = Convert.FromBase64String(şifreleyiciText);
                return MetniBellektenÇöz(buffer, sağlayıcı.Key, sağlayıcı.IV);
            }
        }

        #region Utilities

        private byte[] MetniBelleğeŞifrele(string data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    byte[] toEncrypt = Encoding.Unicode.GetBytes(data);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }

        private string MetniBellektenÇöz(byte[] data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs, Encoding.Unicode))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        #endregion
    }
}
