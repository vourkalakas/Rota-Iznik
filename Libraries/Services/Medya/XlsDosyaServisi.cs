using Core;
using Core.Domain.Medya;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Medya
{
    public partial class XlsDosyaServisi : IXlsDosyaServisi
    {
        protected virtual string MimeTipindenDosyaUzantısınıAl(string mimeTipi)
        {
            if (mimeTipi == null)
                return null;

            string[] parçalar = mimeTipi.Split('/');
            string sonParça = parçalar[parçalar.Length - 1];
            switch (sonParça)
            {
                case "vnd.open":
                    sonParça = "xlsx";
                    break;
                case "xls":
                    sonParça = "xls";
                    break;
            }
            return sonParça;
        }
        protected virtual string XlsLocalYolunuAl(string dosyaAdı)
        {
            return Path.Combine(GenelYardımcı.MapPath("~/content/xls/"), dosyaAdı);
        }
        protected virtual byte[] DosyadanXlsYükle(int xlsId, string mimeTipi)
        {
            string sonParça = MimeTipindenDosyaUzantısınıAl(mimeTipi);
            string dosyaAdı = string.Format("{0}_0.{1}", xlsId.ToString("0000000"), sonParça);
            var dosyaYolu = XlsLocalYolunuAl(dosyaAdı);
            if (!File.Exists(dosyaYolu))
                return new byte[0];
            return File.ReadAllBytes(dosyaYolu);
        }
        protected virtual void DosyayaXlsKaydet(int xlsId, byte[] xlsBinary, string mimeTipi)
        {
            string sonParça = MimeTipindenDosyaUzantısınıAl(mimeTipi);
            string dosyaAdı = string.Format("{0}_0.{1}", xlsId.ToString("0000000"), sonParça);
            File.WriteAllBytes(XlsLocalYolunuAl(dosyaAdı), xlsBinary);
        }

        protected virtual void DosyaSistemindenResimSil(Xls xls)
        {
            if (xls == null)
                throw new ArgumentNullException("xls");

            string sonParça = MimeTipindenDosyaUzantısınıAl(xls.MimeTipi);
            string dosyaAdı = string.Format("{0}_0.{1}", xls.Id.ToString("0000000"), sonParça);
            string dosyaYolu = XlsLocalYolunuAl(dosyaAdı);
            if (File.Exists(dosyaYolu))
            {
                File.Delete(dosyaYolu);
            }
        }
        public virtual byte[] XlsBinaryYükle(Xls xls)
        {
            if (xls == null)
                throw new ArgumentNullException("xls");

            var sonuç = DosyadanXlsYükle(xls.Id, xls.MimeTipi);
            return sonuç;
        }
        public virtual string XlsVarsayılanUrlAl(int hedefBüyüklüğü = 0,
            string siteKonumu = null)
        {
            return "";
        }

        public virtual string XlsUrlAl(Xls xls,
            int hedefBüyüklüğü = 0,
            bool varsayılanResimGöster = true,
            string kaynakKonumu = null)
        {
            string url = string.Empty;
            byte[] xlsBinary = null;
            if (xls != null)
                xlsBinary = XlsBinaryYükle(xls);
            if (xls == null || xlsBinary == null || xlsBinary.Length == 0)
            {
                if (varsayılanResimGöster)
                {
                    url = XlsVarsayılanUrlAl(hedefBüyüklüğü, kaynakKonumu);
                }
                return url;
            }
            /*
            if (xls.Yeni)
            {
                xls = ResimGüncelle(xls.Id,
                    xlsBinary,
                    xls.MimeTipi,
                    xls.SeoDosyaAdı,
                    xls.AltÖznitelik,
                    xls.BaşlıkÖznitelik,
                    false,
                    false);
            }
            */
            var seoFileName = xls.SeoDosyaAdı;

            string sonParça = MimeTipindenDosyaUzantısınıAl(xls.MimeTipi);
            string thumbDosyaAdı;
            if (hedefBüyüklüğü == 0)
            {
                thumbDosyaAdı = !String.IsNullOrEmpty(seoFileName)
                    ? string.Format("{0}_{1}.{2}", xls.Id.ToString("0000000"), seoFileName, sonParça)
                    : string.Format("{0}.{1}", xls.Id.ToString("0000000"), sonParça);
            }
            else
            {
                thumbDosyaAdı = !String.IsNullOrEmpty(seoFileName)
                    ? string.Format("{0}_{1}_{2}.{3}", xls.Id.ToString("0000000"), seoFileName, hedefBüyüklüğü, sonParça)
                    : string.Format("{0}_{1}.{2}", xls.Id.ToString("0000000"), hedefBüyüklüğü, sonParça);
            }
            return url;
        }
        public virtual Xls XlsEkle(byte[] xlsBinary, string mimeTipi, string seoDosyaAdı,
            string altÖznitelik = null, string başlıkÖznitelik = null,
            bool Yeni = true, bool BinaryDoğrula = true)
        {
            mimeTipi = GenelYardımcı.BoşKontrol(mimeTipi);
            mimeTipi = GenelYardımcı.MaksimumUzunlukKontrol(mimeTipi, 20);

            seoDosyaAdı = GenelYardımcı.MaksimumUzunlukKontrol(seoDosyaAdı, 100);

            /*
            if (BinaryDoğrula)
                xlsBinary = ResimDoğrula(resimBinary, mimeTipi);
                */
            var xls = new Xls
            {
                XlsBinary = new byte[0],
                MimeTipi = mimeTipi,
                SeoDosyaAdı = seoDosyaAdı,
                AltÖznitelik = altÖznitelik,
                BaşlıkÖznitelik = başlıkÖznitelik,
                Yeni = Yeni,
            };

            DosyayaXlsKaydet(xls.Id, xlsBinary, mimeTipi);
            return xls;
        }
    }
}
