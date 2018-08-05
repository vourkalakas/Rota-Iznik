using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Güvenlik
{
    public interface IŞifrelemeServisi
    {
        string SaltAnahtarıOluştur(int büyüklük);
        string ŞifreHashOluştur(string şifre, string saltanahtarı, string şifreFormatı = "SHA1");
        string HashOluştur(byte[] data, string hashAlgorithm = "SHA1");
        string TextŞifrele(string düzMetin, string şifrelemeÖzelAnahtarı = "");
        string TextÇöz(string şifreleyiciText, string şifrelemeÖzelAnahtarı = "");
    }
}
