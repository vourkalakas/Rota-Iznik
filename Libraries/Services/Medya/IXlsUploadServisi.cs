
using Core;
using Core.Domain.Medya;

namespace Services.Medya
{
    public partial interface IXlsUploadServisi
    {
        byte[] XlsBinaryYükle(Xls xls);
        
        Xls XlsEkle(byte[] xlsBinary, string mimeTipi, string seoDosyaAdı,
            string altÖznitelik = null, string başlıkÖznitelik = null,
            bool Yeni = true, bool BinaryDoğrula = true);
    }
}
