using System.Collections.Generic;
using Core;
using Core.Domain.Medya;

namespace Services.Medya
{
    public partial interface IResimServisi
    {
        byte[] ResimBinaryYükle(Resim resim);
        string ResimSeAdıAl(string adı);
        string ResimVarsayılanUrlAl(int hedefBüyüklüğü = 0,
            ResimTipi varsayılanResimTipi = ResimTipi.Varlık,
            string kaynakKonumu = null);
        string ResimUrlAl(int resimId,
            int hedefBüyüklüğü = 0,
            bool varsayılanResimGöster = true,
            string kaynakKonumu = null,
            ResimTipi varsayılanResimTipi = ResimTipi.Varlık);
        string ResimUrlAl(Resim resim,
            int hedefBüyüklüğü = 0,
            bool varsayılanResimGöster = true,
            string kaynakKonumu = null,
            ResimTipi varsayılanResimTipi = ResimTipi.Varlık);
        string ThumbYoluAl(Resim resim, int hedefBüyüklüğü = 0, bool varsayılanResimGöster = true);
        Resim ResimAlId(int resimId);
        void ResimSil(Resim resim);
        ISayfalıListe<Resim> ResimleriAl(int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        Resim ResimEkle(byte[] resimBinary, string mimeTipi, string seoDosyaAdı,
            string altÖznitelik = null, string başlıkÖznitelik = null,
            bool Yeni = true, bool BinaryDoğrula = true);
        Resim ResimGüncelle(int resimId,byte[] resimBinary, string mimeTipi, string seoDosyaAdı,
            string altÖznitelik = null, string başlıkÖznitelik = null,
            bool Yeni = true, bool BinaryDoğrula = true);
        Resim SeoDosyaAdıAyarla(int resimId, string seoDosyaAdı);
        byte[] ResimDoğrula(byte[] resimBinary, string mimeTipi);
        bool VeritabanındaDepola { get; set; }
        IDictionary<int, string> ResimHashAl(int[] resimIdleri);
    }
}
