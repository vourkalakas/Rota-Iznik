
using Core;
using Core.Domain.Medya;

namespace Services.Medya
{
    public partial interface IXlsDosyaServisi
    {
        byte[] XlsBinaryYükle(Xls xls);
        string XlsVarsayılanUrlAl(int hedefBüyüklüğü = 0,
             string kaynakKonumu = null);
        string XlsUrlAl(Xls resim,
            int hedefBüyüklüğü = 0,
            bool varsayılanResimGöster = true,
            string kaynakKonumu = null);
        Xls XlsEkle(byte[] xlsBinary, string mimeTipi, string seoDosyaAdı,
            string altÖznitelik = null, string başlıkÖznitelik = null,
            bool Yeni = true, bool BinaryDoğrula = true);
    }
}
