using Core;
using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Core.Domain.Teklif;
using Core.Domain.Genel;
using Services.Yapılandırma;
using Services.Medya;
using Services.Tanımlamalar;
using System.Text;
using iTextSharp.text.html.simpleparser;
using Services.Teklifler;
using System.Globalization;
using Services.Kullanıcılar;
using Services.Yetkililer;
//using System.Web.Mvc;

namespace Services.Klasör
{
    public partial class PdfServisi : IPdfServisi
    {
        CultureInfo ci = new CultureInfo("tr-TR");
        //CultureInfo tr = new CultureInfo("tr-TR");
        private readonly IWorkContext _workContext;
        private readonly PdfAyarları _pdfAyarları;
        private readonly IAyarlarServisi _ayarlarServisi;
        private readonly IResimServisi _resimServisi;
        private readonly IMusteriServisi _musteriServisi;
        private readonly IBagliTeklifOgesiServisi _bagliTeklifOgesi;
        private readonly IKullanıcıServisi _kullanıcıServisi;
        private readonly IYetkiliServisi _yetkiliServisi;
        private readonly ITeklifServisi _teklifServisi;

        public PdfServisi(IWorkContext workContext,
            PdfAyarları pdfAyarları,
            IAyarlarServisi ayarlarServisi,
            IResimServisi resimServisi,
            IMusteriServisi musteriServisi,
            IBagliTeklifOgesiServisi bagliTeklifOgesi,
            IKullanıcıServisi kullanıcıServisi,
            IYetkiliServisi yetkiliServisi,
            ITeklifServisi teklifServisi
            )
        {
            this._workContext = workContext;
            this._pdfAyarları = pdfAyarları;
            this._ayarlarServisi = ayarlarServisi;
            this._resimServisi = resimServisi;
            this._musteriServisi = musteriServisi;
            this._bagliTeklifOgesi = bagliTeklifOgesi;
            this._kullanıcıServisi = kullanıcıServisi;
            this._yetkiliServisi = yetkiliServisi;
            this._teklifServisi = teklifServisi;
        }

        protected virtual Font FontAl()
        {
            return FontAl(_pdfAyarları.FontDosyaAdı);
        }
        protected virtual Font FontAl(string fontDosyaAdı)
        {
            if (fontDosyaAdı == null)
                throw new ArgumentNullException("fontFileName");

            string fontYolu = Path.Combine(GenelYardımcı.MapPath("~/App_Data/Pdf/"), fontDosyaAdı);
            var baseFont = BaseFont.CreateFont(fontYolu, "windows-1254", BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }
        public virtual string TeklifPdfOlustur(Teklif teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");

            string dosyaAdı = string.Format("teklif_{0}_{1}.pdf", teklif.Id, GenelYardımcı.RastgeleTamSayıÜret(4));
            string dosyaYolu = Path.Combine(GenelYardımcı.MapPath("~/content/files/pdf"), dosyaAdı);
            using (var fileStream = new FileStream(dosyaYolu, FileMode.Create))
            {
                var teklifler = new List<Teklif>();
                teklifler.Add(teklif);
                TeklifPdfOlustur(fileStream, teklifler);
            }

            return dosyaYolu;
        }
        public virtual void TeklifPdfOlustur(Stream stream, IList<Teklif> teklifler)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (teklifler == null)
                throw new ArgumentNullException("teklifler");

            var sayfaBüyüklüğü = PageSize.A4;

            if (_pdfAyarları.HarfSayfaBüyüklüğüEtkin)
            {
                sayfaBüyüklüğü = PageSize.LETTER;
            }

            StringBuilder sb = new StringBuilder();
            var doc = new Document(sayfaBüyüklüğü);

            StyleSheet styles = new StyleSheet();
            FontFactory.Register(GenelYardımcı.MapPath("~/App_Data/Pdf/arial.ttf"), "Garamond");   // just give a path of arial.ttf 
            styles.LoadTagStyle("body", "face", "Garamond");
            styles.LoadTagStyle("body", "encoding", "Identity-H");
            int teklifCount = teklifler.Count;
            foreach (var teklif in teklifler)
            {
                var pdfAyarları = _ayarlarServisi.AyarYükle<PdfAyarları>();
                var firmaId = teklif.FirmaId;
                string musteriAdı = "";
                var musteriGrubu = 1;
                if (firmaId != 0)
                    musteriAdı = _musteriServisi.MusteriAlId(firmaId).Adı;

                #region Header

                //logo
                var logoResmi = _resimServisi.ResimAlId(pdfAyarları.LogoResimId);
                var logoPath = _resimServisi.ThumbYoluAl(logoResmi, 0, false);
                var logoMevcut = logoResmi != null;
                string resim = "<img src=\"" + logoPath + "\" />";

                string yetkili = "";
                string musteri = "";
                if (teklif.YetkiliId > 0)
                    yetkili = _yetkiliServisi.YetkiliAlId(teklif.YetkiliId).Adı + " " + _yetkiliServisi.YetkiliAlId(teklif.YetkiliId).Soyadı;
                if (teklif.FirmaId > 0)
                    musteri = _musteriServisi.MusteriAlId(teklif.FirmaId).Adı;

                sb.Append("<table style=\"font-family:Arial; font-size:6px;\"><tr><td rowspan=\"6\" colspan=\"4\">" + resim +
                        "</td><td>Müşteri Adı:</td><td>" + musteri
                        + "</td></tr><tr><td>İlgili Kişi</td><td>" + yetkili
                        + "</td></tr><tr><td>Hazırlayan:</td><td>" + ""
                        + "</td></tr><tr><td>Toplantı Adı:</td><td>" + teklif.ToplantıAdı
                        + "</td></tr><tr><td>Organizasyon Tarihi:</td><td>" + teklif.BaslamaTarihi + " - " + teklif.BitisTarihi
                        + "</td></tr><tr><td>Yayınlanma Tarihi:</td><td>" + DateTime.Now.Date.ToShortDateString()
                        + "</td></tr></table>");

                sb.Append("<table style=\"font-family:Arial; font-size:6px;\"><tr><td>Mekan Adı: " + teklif.MekanAdı
                    + "</td></tr><tr><td>Konum: " + teklif.Konum
                    + "</td></tr><tr><td>Kod No: " + teklif.Kod
                    + "</td></tr></table>");
                bool yurtdisi = false;

                sb.Append("<br />");
                sb.Append("<table border = '1' bgcolor=\"#3E6BE5\" style=\"font-family:Arial; font-size:6px; color:#fff;\"><tr style='text-align:center;'><th>BAŞLIK</th><th>ACIKLAMA</th><th>KİŞİ/ADET</th><th>GÜN</th><th>BİRİM FİYAT</th><th>TOPLAM</th></tr></table>");
                sb.Append("<table border = '1' style=\"font-family:Arial; font-size:6px;\">");
                decimal araToplam = 0, araToplamDolar = 0, araToplamEuro = 0, araToplamKonaklama = 0,
                    araToplamKonaklamaDolar = 0, araToplamKonaklamaEuro = 0, araToplamDiger = 0, araToplamDolarDiger = 0,
                    araToplamEuroDiger = 0, dipToplam = 0, dipToplamDolar = 0, dipToplamEuro = 0, hizmet = 0, hizmetDolar = 0,
                    hizmetEuro = 0, genelToplam = 0, genelToplamDolar = 0, genelToplamEuro = 0, KDV8 = 0, KDV8Dolar = 0, KDV8Euro = 0,
                    KDV18 = 0, KDV18Dolar = 0, KDV18Euro = 0, genelToplamKDVli = 0, genelToplamKDVliDolar = 0, genelToplamKDVliEuro = 0;
                int count = _bagliTeklifOgesi.BagliTeklifOgesiAlTeklifId(teklif.Id).Count, hb = 0;
                if (teklif.UlkeId != 1)
                    yurtdisi = true;
                List<int> parabirimleri = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    var ogeler = _bagliTeklifOgesi.BagliTeklifOgesiAlTeklifId(teklif.Id);
                    var oge = ogeler[i];
                    BagliTeklifOgesi oge2;
                    if (count > i + 1)
                        oge2 = ogeler[i + 1];
                    else
                        oge2 = ogeler[i];
                    string adı = "", açıklama = "", tparent = "", kurum = "", gelir = "", parabirimi = "";
                    decimal alışBirimFiyat = 0, alışBirimFiyatDolar = 0, alışBirimFiyatEuro = 0,
                        satışBirimFiyat = 0, satışBirimFiyatDolar = 0, satışBirimFiyatEuro = 0,
                        alışFiyat = 0, alışFiyatDolar = 0, alışFiyatEuro = 0, satışFiyat = 0,
                        satışFiyatDolar = 0, satışFiyatEuro = 0, toplamFiyat = 0, toplamFiyatDolar = 0,
                        toplamFiyatEuro = 0, kar = 0, karDolar = 0, karEuro = 0;
                    int adet = 0, gun = 0, kdv = 0, vparent = 0, parabirimiDeger = 0, teklifId = 0, ogeId = 0;
                    bool konaklama = false;

                    alışBirimFiyat = oge.AlisBirimFiyat;
                    alışBirimFiyatDolar = oge.AlisBirimFiyatDolar;
                    alışBirimFiyatEuro = oge.AlisBirimFiyatEuro;
                    satışBirimFiyat = oge.SatisBirimFiyat;
                    satışBirimFiyatDolar = oge.SatisBirimFiyatDolar;
                    satışBirimFiyatEuro = oge.SatisBirimFiyatEuro;
                    alışFiyat = oge.AlisFiyat;
                    alışFiyatDolar = oge.AlisFiyatDolar;
                    alışFiyatEuro = oge.AlisFiyatEuro;
                    satışFiyat = oge.SatisFiyat;
                    satışFiyatDolar = oge.SatisFiyatDolar;
                    satışFiyatEuro = oge.SatisFiyatEuro;
                    toplamFiyat = oge.ToplamFiyat;
                    toplamFiyatDolar = oge.ToplamFiyatDolar;
                    toplamFiyatEuro = oge.ToplamFiyatEuro;
                    kar = oge.Kar;
                    karDolar = oge.KarDolar;
                    karEuro = oge.KarEuro;
                    gelir = oge.Gelir;
                    adet = oge.Adet;
                    gun = oge.Gun;
                    kdv = oge.Kdv;
                    vparent = oge.Vparent;
                    parabirimiDeger = oge.Parabirimi;
                    teklifId = oge.TeklifId;
                    ogeId = oge.Id;
                    adı = oge.Adı;
                    açıklama = oge.Aciklama;
                    tparent = oge.Tparent;
                    kurum = oge.Kurum;
                    parabirimi = parabirimiDeger == 1 ? "TL" : parabirimiDeger == 2 ? "$" : "€";
                    //bool 
                    if (parabirimleri.Count == 0)
                        parabirimleri.Add(Convert.ToInt32(parabirimiDeger));

                    for (int j = 0; j < parabirimleri.Count; j++)
                    {
                        int p = Convert.ToInt32(parabirimiDeger);
                        if (parabirimleri.Contains(p))
                            break;
                        else
                            parabirimleri.Add(p);
                    }
                    if (kdv == 8)
                        konaklama = true;
                    if (konaklama)
                    {
                        araToplamKonaklama += satışFiyat;
                        araToplamKonaklamaDolar += satışFiyatDolar;
                        araToplamKonaklamaEuro += satışFiyatEuro;
                    }
                    else
                    {
                        araToplamDiger += satışFiyat;
                        araToplamDolarDiger += satışFiyatDolar;
                        araToplamEuroDiger += satışFiyatEuro;
                    }
                    araToplam += satışFiyat;
                    araToplamDolar += satışFiyatDolar;
                    araToplamEuro += satışFiyatEuro;

                    if (i == 0)
                    {
                        sb.Append("<tr><th colspan=\"6\"  bgcolor=\"#3E6BE5\" style=\"color:#fff;\">" + tparent + "</th></tr>");
                    }
                    sb.Append("<tr style='text-align:right;'><td style='text-align:left;'>" + adı + "</td><td style='text-align:left;'>" + açıklama + "</td><td style='text-align:center;'>" + adet + "</td><td style='text-align:center;'>"
                            + gun + "</td><td>" + (parabirimiDeger == 1 ? (satışBirimFiyat.ToString("0,0.00", ci) + parabirimi) : parabirimiDeger == 2 ? (satışBirimFiyatDolar.ToString("0,0.00", ci) + parabirimi) : (satışBirimFiyatEuro.ToString("0,0.00", ci) + parabirimi))
                            + "</td><td>" + (parabirimiDeger == 1 ? (satışFiyat.ToString("0,0.00", ci) + parabirimi) : parabirimiDeger == 2 ? (satışFiyatDolar.ToString("0,0.00", ci) + parabirimi) : (satışFiyatEuro.ToString("0,0.00", ci) + parabirimi)) + "</td></tr>");

                    if (vparent != oge2.Vparent)
                    {
                        sb.Append("<tr style='text-align:center;'><th colspan=\"5\" style='text-align:right;' border='0'>" + tparent + "</th><td style='text-align:right;' bgcolor=\"#30e5d3\">" + ((parabirimi == "TL") ? araToplam.ToString("0,0.00", ci) + "TL" : (parabirimi == "$") ? araToplamDolar.ToString("0,0.00", ci) + "$" : araToplamEuro.ToString("0,0.00", ci) + "€") + "</td></tr><tr><td colspan=\"6\" border='0'> </td></tr>");
                        sb.Append("<tr><th colspan=\"6\" bgcolor=\"#3E6BE5\" style=\"color:#fff;\">" + oge2.Tparent + "</th></tr>");
                        araToplam = 0;
                        araToplamDolar = 0;
                        araToplamEuro = 0;
                    }
                    if (i == count - 1)
                        sb.Append("<tr style='text-align:center;'><th colspan=\"5\" style='text-align:right;' border='0'>" + tparent + "</th><td style='text-align:right;' bgcolor=\"#30e5d3\">" + ((parabirimi == "TL") ? araToplam.ToString("0,0.00", ci) + "TL" : (parabirimi == "$") ? araToplamDolar.ToString("0,0.00", ci) + "$" : araToplamEuro.ToString("0,0.00", ci) + "€") + "</td></tr><tr><td colspan=\"6\" border='0'> </td></tr>");
                }
                if (teklif.HizmetBedeli.ToString() != string.Empty || teklif.HizmetBedeli.ToString() != "")
                    hb = teklif.HizmetBedeli;

                dipToplam = araToplamDiger + araToplamKonaklama;
                dipToplamDolar = araToplamDolarDiger + araToplamKonaklamaDolar;
                dipToplamEuro = araToplamEuroDiger + araToplamKonaklamaEuro;
                hizmet = (araToplamKonaklama + araToplamDiger) / 100 * hb;
                hizmetDolar = (araToplamKonaklamaDolar + araToplamDolarDiger) / 100 * hb;
                hizmetEuro = (araToplamKonaklamaEuro + araToplamEuroDiger) / 100 * hb;
                genelToplam = dipToplam + hizmet;
                genelToplamDolar = dipToplamDolar + hizmetDolar;
                genelToplamEuro = dipToplamEuro + hizmetEuro;
                KDV8 = yurtdisi ? 0 : araToplamKonaklama * 8 / 100;
                KDV8Dolar = yurtdisi ? 0 : araToplamKonaklamaDolar * 8 / 100;
                KDV8Euro = yurtdisi ? 0 : araToplamKonaklamaEuro * 8 / 100;
                KDV18 = yurtdisi ? 0 : araToplamDiger * 18 / 100 + hizmet * 18 / 100;
                KDV18Dolar = yurtdisi ? 0 : araToplamDolarDiger * 18 / 100 + hizmetDolar * 18 / 100;
                KDV18Euro = yurtdisi ? 0 : araToplamEuroDiger * 18 / 100 + hizmetEuro * 18 / 100;
                genelToplamKDVli = yurtdisi ? genelToplam : genelToplam + KDV18 + KDV8;
                genelToplamKDVliDolar = yurtdisi ? genelToplamDolar : genelToplamDolar + KDV18Dolar + KDV8Dolar;
                genelToplamKDVliEuro = yurtdisi ? genelToplamEuro : genelToplamEuro + KDV18Euro + KDV8Euro;

                sb.Append("</table>");
                sb.Append("<table border = '1' style=\"font-family:Arial; font-size:6px;\">" +
                        "<tr><th colspan=\"6\"></th></tr><tr style='text-align:right;'>" +
                        "<td style='text-align:left;'>Konaklama Toplamı</td><td colspan=\"2\"></td><td>"
                        + araToplamKonaklamaEuro.ToString("0,0.00", ci) + "€" + "</td><td>" + araToplamKonaklamaDolar.ToString("0,0.00", ci)
                        + "$" + "</td><td>" + araToplamKonaklama.ToString("0,0.00", ci) + "TL" + "</td></tr><tr style='text-align:right;'>" +
                        "<td style='text-align:left;'>Diğer Toplam</td><td colspan=\"2\"></td><td>"
                        + araToplamEuroDiger.ToString("0,0.00", ci) + "€" + "</td><td>" + araToplamDolarDiger.ToString("0,0.00", ci) + "$"
                        + "</td><td>" + araToplamDiger.ToString("0,0.00", ci) + "TL" + "</td></tr><tr><td colspan=\"6\"> </td></tr></table>");
                
                for (int i = 0; i < parabirimleri.Count; i++)
                {
                    string parabirimi2 = parabirimleri[i] == 1 ? "TL" : parabirimleri[i] == 2 ? "$" : "€";
                    string parabirimi3 = parabirimleri[i] == 1 ? "TL" : parabirimleri[i] == 2 ? "DOLAR" : "EURO";
                    string color = parabirimleri[i] == 1 ? "#C64333" : parabirimleri[i] == 2 ? "#00A65A" : "#00C0EF";
                    string diptoplam = parabirimleri[i] == 1 ? dipToplam.ToString("0,0.00", ci) : parabirimleri[i] == 2 ? dipToplamDolar.ToString("0,0.00", ci) : dipToplamEuro.ToString("0,0.00", ci);
                    string hzmt = parabirimleri[i] == 1 ? hizmet.ToString("0,0.00", ci) : parabirimleri[i] == 2 ? hizmetDolar.ToString("0,0.00", ci) : hizmetEuro.ToString("0,0.00", ci);
                    string kdv8 = parabirimleri[i] == 1 ? KDV8.ToString("0,0.00", ci) : parabirimleri[i] == 2 ? KDV8Dolar.ToString("0,0.00", ci) : KDV8Euro.ToString("0,0.00", ci);
                    string kdv18 = parabirimleri[i] == 1 ? KDV18.ToString("0,0.00", ci) : parabirimleri[i] == 2 ? KDV18Dolar.ToString("0,0.00", ci) : KDV18Euro.ToString("0,0.00", ci);
                    string geneltoplamkdvli = parabirimleri[i] == 1 ? genelToplamKDVli.ToString("0,0.00", ci) : parabirimleri[i] == 2 ? genelToplamKDVliDolar.ToString("0,0.00", ci) : genelToplamKDVliEuro.ToString("0,0.00", ci);
                    string geneltoplam = parabirimleri[i] == 1 ? genelToplam.ToString("0,0.00", ci) : parabirimleri[i] == 2 ? genelToplamDolar.ToString("0,0.00", ci) : genelToplamEuro.ToString("0,0.00", ci);
                    sb.Append("<table border = '1' bgcolor=\"" + color + "\"  style=\"font-family:Arial; font-size:6px; color:#fff;\">" +
                    "<tr style='text-align:right;'><td style='text-align:left;'>" + parabirimi3 + " Toplam</td><td colspan=\"4\" ></td><td>" + diptoplam + parabirimi2
                    + "<tr style='text-align:right;'><td style='text-align:left;'>" + parabirimi3 + " Acenta Hizmet Bedeli %" + hb.ToString() + "</td><td colspan=\"4\" ></td><td>" + hzmt + parabirimi2
                    + "<tr style='text-align:right;'><td style='text-align:left;'>" + parabirimi3 + " Genel Toplam" + hb.ToString() + "</td><td colspan=\"4\" ></td><td>" + geneltoplam + parabirimi2
                    + "<tr style='text-align:right;'><td style='text-align:left;'>" + parabirimi3 + " KDV %8</td><td colspan=\"4\" ></td><td>" + kdv8 + parabirimi2
                    + "<tr style='text-align:right;'><td style='text-align:left;'>" + parabirimi3 + " KDV %18</td><td colspan=\"4\" ></td><td>" + kdv18 + parabirimi2
                    + "<tr style='text-align:right;'><td style='text-align:left;'>" + parabirimi3 + " KDV'li Genel Toplam</td><td colspan=\"4\" ></td><td>" + geneltoplamkdvli + parabirimi2
                     + "</td></tr><tr><td bgcolor=\"#ffffff\" colspan=\"6\"></td></tr></table>");
                }
                //Döviz Kuru
                sb.Append("<table border = '1' bgcolor=\"#30e5d3\" style=\"font-family:Arial; font-size:6px;\"><tr><th align=\"center\" colspan=\"6\">" + DateTime.Now.Date.ToShortDateString() + " tarihindeki TCMB efektif satış döviz kurları baz alınarak hesaplanmıştır.<br>1 Euro/" + teklif.KurDolar.ToString("N04", ci) + " TL, 1 Dolar/" + teklif.KurDolar.ToString("N04", ci) + " TL <br><p style=\"font-family:Arial; font-size:8px; color:#ff0000;\">*Organizasyonun gerçekleştiği günkü TCMB efektif satış kuru baz alınarak faturalandırılacaktır.</p></th></tr></table>");
                #endregion
            }
            StringReader sr = new StringReader(sb.ToString());
            HTMLWorker htmlparser = new HTMLWorker(doc, null, styles);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();
            htmlparser.Parse(sr);
            doc.Close();
        }
        public virtual string TeklifRaporOlustur(Teklif teklif)
        {
            if (teklif == null)
                throw new ArgumentNullException("teklif");

            string dosyaAdı = string.Format("rapor_{0}_{1}.pdf", teklif.Id, GenelYardımcı.RastgeleTamSayıÜret(4));
            string dosyaYolu = Path.Combine(GenelYardımcı.MapPath("~/content/files/pdf"), dosyaAdı);
            using (var fileStream = new FileStream(dosyaYolu, FileMode.Create))
            {
                var teklifler = new List<Teklif>();
                teklifler.Add(teklif);
                TeklifRaporOlustur(fileStream, teklifler);
            }

            return dosyaYolu;
        }
        public virtual void TeklifRaporOlustur(Stream stream, IList<Teklif> teklifler)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (teklifler == null)
                throw new ArgumentNullException("teklifler");

            var sayfaBüyüklüğü = PageSize.A4;

            if (_pdfAyarları.HarfSayfaBüyüklüğüEtkin)
            {
                sayfaBüyüklüğü = PageSize.LETTER;
            }
            foreach (var teklif in teklifler)
            {
                var pdfAyarları = _ayarlarServisi.AyarYükle<PdfAyarları>();
                string kurumadi = "", teklifno = teklif.Id.ToString(), ilgilikisi = "", kurumaciklama = "", hazırlayan = "", aciklama = "", konum = "", kodno = "", sparabirimi = "";
                decimal kurEuro, kurDolar, tlTutar = 0, tlAlis = 0, euroTutar = 0, dolarTutar = 0, tlToplam = 0
                    , dolarToplam = 0, euroToplam = 0, tlToplamK = 0, dolarToplamK = 0, euroToplamK = 0
                    , euroToplamG = 0, tlToplamG = 0, euroHizmet = 0, tlHizmet = 0, euroHaftasonu = 0
                    , tlHaftasonu = 0, tlKDV8 = 0, euroKDV8 = 0, tlKDV18 = 0, euroKDV18 = 0, tlKDVG = 0
                    , euroKDVG = 0, hb = 0, dolarHaftasonu = 0, dolarHizmet = 0, dolarToplamG = 0, dolarKDV8 = 0
                    , dolarKDV18 = 0, dolarKDVG = 0, tlGelir = 0, tlKar = 0;

                kurumadi = teklif.FirmaId>0? _musteriServisi.MusteriAlId(teklif.FirmaId).Adı:"";

                kurEuro = teklif.KurEuro>0? teklif.KurEuro:1;
                kurDolar = teklif.KurDolar>0? teklif.KurDolar:1;
                DateTime kurTarih = DateTime.Now;
                var teklif2 = _teklifServisi.TeklifAlId(teklif.OrijinalTeklifId);

                bool yurtdisi = false;
                ilgilikisi = teklif.YetkiliId>0? _yetkiliServisi.YetkiliAlId(teklif.YetkiliId).Adı+" "+ _yetkiliServisi.YetkiliAlId(teklif.YetkiliId).Soyadı : "";
                //kurumaciklama = _musteriServisi.MusteriAlId(teklif.)
                konum = teklif.Konum;
                kodno = teklif.Kod;
                if (teklif.UlkeId != 1)
                    yurtdisi = true;

                hazırlayan = teklif.HazırlayanId > 0 ? _kullanıcıServisi.KullanıcıAlId(teklif.HazırlayanId).TamAdAl() : "";

                var logoResmi = _resimServisi.ResimAlId(pdfAyarları.LogoResimId);
                var logoPath = _resimServisi.ThumbYoluAl(logoResmi, 0, false);
                var logoMevcut = logoResmi != null;
                string resim = "<img src=\"" + logoPath + "\" />",parabirimi;

                StringBuilder sb = new StringBuilder();//
                var doc = new Document(sayfaBüyüklüğü);

                StyleSheet styles = new StyleSheet();
                FontFactory.Register(GenelYardımcı.MapPath("~/App_Data/Pdf/arial.ttf"), "Garamond");   // just give a path of arial.ttf 
                styles.LoadTagStyle("body", "face", "Garamond");
                styles.LoadTagStyle("body", "encoding", "Identity-H");
                int teklifCount = teklifler.Count;

                sb.Append("<table border = '1' bgcolor=\"#3E6BE5\" style=\"font-family:Arial; font-size:6px; color:#fff;\"><tr><th colspan=\"4\">Teklif</th><th colspan=\"2\">Alış Fiyatı</th><th colspan=\"2\">Satış Fiyat</th><th colspan=\"2\">Kar/Yüzde Oranı</th><th colspan=\"3\"></th></tr><tr><th>Aciklama</th><th>KURUM</th><th>KİŞİ/ADET</th><th>GÜN</th><th>BİRİM FİYAT</th><th>TUTAR</th><th>BİRİM FİYAT</th><th>TUTAR</th><th>KAR</th><th>GELIR</th><th>EURO TOPLAM</th><th>DOLAR TOPLAM</th><th>TL TOPLAM</th></tr></table>");
                sb.Append("<table border = '1' style=\"font-family:Arial; font-size:6px;\">");
                decimal alist = 0, satist = 0, kart = 0, gelirt = 0, geliryuzde = 0, alistk = 0, satistk = 0, kartk = 0, gelirtk = 0, geliryuzdek = 0;

                int count = _bagliTeklifOgesi.BagliTeklifOgesiAlTeklifId(teklif.Id).Count;
                //int hb = 0;
                if (teklif.UlkeId != 1)
                    yurtdisi = true;
                List<int> parabirimleri = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    var ogeler = _bagliTeklifOgesi.BagliTeklifOgesiAlTeklifId(teklif.Id);
                    var oge = ogeler[i];
                    BagliTeklifOgesi oge2;
                    if (count > i + 1)
                        oge2 = ogeler[i + 1];
                    else
                        oge2 = ogeler[i];

                    decimal tutar = oge.SatisFiyat;
                    decimal alis = oge.AlisFiyat;
                    decimal gelir = Convert.ToDecimal(oge.Gelir);
                    decimal kar = oge.Kar;
                    int parabirimiDeger = oge.Parabirimi;
                    parabirimi = parabirimiDeger == 1 ? "TL" : parabirimiDeger == 2 ? "$" : "€";
                    int konaklama = 0;

                    if (parabirimleri.Count == 0)
                        parabirimleri.Add(Convert.ToInt32(parabirimiDeger));

                    for (int j = 0; j < parabirimleri.Count; j++)
                    {
                        int p = Convert.ToInt32(parabirimiDeger);
                        if (parabirimleri.Contains(p))
                            break;
                        else
                            parabirimleri.Add(p);
                    }

                    if (oge.Kdv == 8)
                        konaklama = 1;
                    else
                        konaklama = 0;
                    if (parabirimiDeger == 1)
                    {
                        sparabirimi = "TL";
                        tlTutar = tutar;
                        euroTutar = Convert.ToDecimal((tutar  / kurEuro));
                        dolarTutar = Convert.ToDecimal((tutar  / kurDolar));
                        tlAlis = alis;
                        tlGelir = gelir;
                        tlKar = kar;
                    }
                    if (parabirimiDeger == 2)
                    {
                        sparabirimi = "$";
                        tlTutar = Convert.ToDecimal((tutar * kurDolar ));
                        euroTutar = Convert.ToDecimal((tlTutar  / kurEuro));
                        dolarTutar = tutar;
                        tlAlis = Convert.ToDecimal((alis * kurDolar ));
                        tlGelir = Convert.ToDecimal((gelir * kurDolar ));
                        tlKar = Convert.ToDecimal((kar * kurDolar ));
                    }
                    if (parabirimiDeger == 3)
                    {
                        sparabirimi = "€";
                        tlTutar = Convert.ToDecimal((tutar * kurEuro ));
                        euroTutar = tutar;
                        dolarTutar = Convert.ToDecimal((tutar  / kurDolar));
                        tlAlis = Convert.ToDecimal((alis * kurEuro ));
                        tlGelir = Convert.ToDecimal((gelir * kurEuro ));
                        tlKar = Convert.ToDecimal((kar * kurEuro ));
                    }
                    if (konaklama == 1)
                    {
                        alistk += tlAlis;
                        satistk += tlTutar;
                        gelirtk += tlGelir;
                        kartk += tlKar;
                        geliryuzdek = satistk > 0? kartk / satistk * 100:0;
                        geliryuzdek = Math.Round(geliryuzdek, 2);
                        tlToplamK += tlTutar;
                        dolarToplamK += dolarTutar;
                        euroToplamK += euroTutar;
                    }
                    else
                    {
                        alist += tlAlis;
                        satist += tlTutar;
                        gelirt += tlGelir;
                        kart += tlKar;
                        geliryuzde = satist > 0? kart / satist * 100:0;
                        geliryuzde = Math.Round(geliryuzde, 2);
                        tlToplam += tlTutar;
                        dolarToplam += dolarTutar;
                        euroToplam += euroTutar;
                    }
                    if (oge.Aciklama != string.Empty || oge.Aciklama != "")
                        aciklama = " (" + oge.Aciklama + ") ";
                    else
                        aciklama = "";
                    if (i == 0)
                    {
                        sb.Append("<tr><th colspan=\"13\"  bgcolor=\"#3E6BE5\" style=\"color:#fff;\">" + oge.Tparent + aciklama + "</th></tr>");
                    }
                    tlTutar = Math.Round(tlTutar, 2);
                    dolarTutar = Math.Round(dolarTutar, 2);
                    euroTutar = Math.Round(euroTutar, 2);

                    int alisadet =oge.AlisAdet;
                    int satisadet =oge.Adet;
                    int adet = alisadet > 0 ? alisadet : satisadet;

                    sb.Append("<tr><td>" + oge.Adı + "</td><td>" + oge.Kurum + "</td><td>" + adet + "</td><td>" + oge.Gun + "</td><td>" + oge.AlisBirimFiyat + sparabirimi + "</td><td>" + oge.AlisFiyat + sparabirimi + "</td><td>" + oge.SatisBirimFiyat + sparabirimi + "</td><td>" + oge.SatisFiyat + sparabirimi + "</td><td>" + oge.Kar + sparabirimi + "</td><td>" + oge.Gelir + "%" + "</td><td>" + euroTutar.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarTutar.ToString("0,0.00", ci) + "$" + "</td><td>" + tlTutar.ToString("0,0.00", ci) + "TL" + "</td></tr>");
                    if (oge.Vparent != oge2.Vparent)
                    {
                        sb.Append("<tr><th colspan=\"13\"  bgcolor=\"#3E6BE5\" style=\"color:#fff;\">" + oge2.Tparent + aciklama + "</th></tr>");
                    }

                }
                if (teklif.HizmetBedeli != 0 || teklif.HizmetBedeli.ToString() != "")
                    hb = Convert.ToDecimal(teklif.HizmetBedeli);

                euroHaftasonu = euroToplamK + euroToplam;
                dolarHaftasonu = dolarToplamK + dolarToplam;
                tlHaftasonu = tlToplamK + tlToplam;
                euroHizmet =hb>0? (euroToplamK + euroToplam) / 100 * hb: (euroToplamK + euroToplam) / 100;
                dolarHizmet = hb > 0 ? (dolarToplamK + dolarToplam) / 100 * hb : (dolarToplamK + dolarToplam) / 100;
                tlHizmet = hb > 0 ? (tlToplamK + tlToplam) / 100 * hb : (tlToplamK + tlToplam) / 100;
                euroToplamG = euroHaftasonu + euroHizmet;
                dolarToplamG = dolarHaftasonu + dolarHizmet;
                tlToplamG = tlHaftasonu + tlHizmet;
                tlKDV8 = yurtdisi ? 0 : tlToplamK * 8 / 100;
                euroKDV8 = yurtdisi ? 0 : euroToplamK * 8 / 100;
                dolarKDV8 = yurtdisi ? 0 : dolarToplamK * 8 / 100;
                tlKDV18 = yurtdisi ? 0 : tlToplam * 18 / 100 + (tlHizmet * 18 / 100);
                euroKDV18 = yurtdisi ? 0 : euroToplam * 18 / 100 + (euroHizmet * 18 / 100);
                dolarKDV18 = yurtdisi ? 0 : dolarToplam * 18 / 100 + (dolarHizmet * 18 / 100);
                tlKDVG = yurtdisi ? 0 : tlToplamG + tlKDV8 + tlKDV18;
                euroKDVG = yurtdisi ? 0 : euroToplamG + euroKDV8 + euroKDV18;
                dolarKDVG = yurtdisi ? 0 : dolarToplamG + dolarKDV8 + dolarKDV18;

                tlToplam = Math.Round(tlToplam, 2);
                dolarToplam = Math.Round(dolarToplam, 2);
                euroToplam = Math.Round(euroToplam, 2);
                tlToplamK = Math.Round(tlToplamK, 2);
                dolarToplamK = Math.Round(dolarToplamK, 2);
                euroToplamK = Math.Round(euroToplamK, 2);
                euroToplamG = Math.Round(euroToplamG, 2);
                dolarToplamG = Math.Round(dolarToplamG, 2);
                tlToplamG = Math.Round(tlToplamG, 2);
                euroHizmet = Math.Round(euroHizmet, 2);
                dolarHizmet = Math.Round(dolarHizmet, 2);
                tlHizmet = Math.Round(tlHizmet, 2);
                euroHaftasonu = Math.Round(euroHaftasonu, 2);
                dolarHaftasonu = Math.Round(dolarHaftasonu, 2);
                tlHaftasonu = Math.Round(tlHaftasonu, 2);
                tlKDV8 = Math.Round(tlKDV8, 2);
                euroKDV8 = Math.Round(euroKDV8, 2);
                dolarKDV8 = Math.Round(dolarKDV8, 2);
                tlKDV18 = Math.Round(tlKDV18, 2);
                euroKDV18 = Math.Round(euroKDV18, 2);
                dolarKDV18 = Math.Round(dolarKDV18, 2);
                tlKDVG = Math.Round(tlKDVG, 2);
                euroKDVG = Math.Round(euroKDVG, 2);
                dolarKDVG = Math.Round(dolarKDVG, 2);

                sb.Append("</table>");
                sb.Append("<table border = '1' style=\"font-family:Arial; font-size:6px;\"><tr><th colspan=\"13\"> </th></tr><tr><td colspan=\"2\">Konaklama Toplamı</td><td colspan=\"3\"><td>" + alistk.ToString("0,0.00", ci) + sparabirimi + "</td><td></td><td>" + satistk.ToString("0,0.00", ci) + sparabirimi + "</td><td>" + kartk.ToString("0,0.00", ci) + sparabirimi + "</td><td>" + geliryuzdek + "%" + "</td><td>" + euroToplamK.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarToplamK.ToString("0,0.00", ci) + "$" + "</td><td>" + tlToplamK.ToString("0,0.00", ci) + "TL" + "</td></tr>");
                sb.Append("<tr><td colspan=\"2\">Diğer Toplam</td><td colspan=\"3\"><td>" + alist.ToString("0,0.00", ci) + sparabirimi + "</td><td></td><td>" + satist.ToString("0,0.00", ci) + sparabirimi + "</td><td>" + kart.ToString("0,0.00", ci) + sparabirimi + "</td><td>" + geliryuzde + "%" + "</td><td>" + euroToplam.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarToplam.ToString("0,0.00", ci) + "$" + "</td><td>" + tlToplam.ToString("0,0.00", ci) + "TL" + "</td></tr>");
                sb.Append("<tr><td colspan=\"2\">Toplam (Hizmet Dahil)</td><td colspan=\"3\"><td>" + (alist + alistk).ToString("0,0.00", ci) + "TL" + "</td><td></td><td>" + (satist + satistk + tlHizmet).ToString("0,0.00", ci) + "TL" + "</td><td>" + (kart + kartk + tlHizmet).ToString("0,0.00", ci) + "TL" + "</td><td>" + ((satist + satistk + tlHizmet)>0?(kart + kartk + tlHizmet) / (satist + satistk + tlHizmet) * 100:0).ToString("0,0.00", ci) + "%" + "</td><td>" + euroHaftasonu.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarHaftasonu.ToString("0,0.00", ci) + "$" + "</td><td>" + tlHaftasonu.ToString("0,0.00", ci) + "TL" + "</td></tr></table>");
                sb.Append("<table border = '1' bgcolor=\"#3E6BE5\"  style=\"font-family:Arial; font-size:6px; color:#fff;\"><tr><th colspan=\"13\"></th></tr><tr><td colspan=\"13\" ></td></tr><tr><td colspan=\"2\">Toplam</td><td colspan=\"8\" ></td><td>" + euroHaftasonu.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarHaftasonu.ToString("0,0.00", ci) + "$" + "</td><td>" + tlHaftasonu.ToString("0,0.00", ci) + "TL" + "</td></tr><tr><td colspan=\"2\">Hizmet Bedeli " + hb.ToString() + "%</td><td colspan=\"8\" ></td><td>" + euroHizmet.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarHizmet.ToString("0,0.00", ci) + "$" + "</td><td>" + tlHizmet.ToString("0,0.00", ci) + "TL" + "</td></tr><tr><td colspan=\"2\">Genel Toplam</td><td colspan=\"8\" ></td><td>" + euroToplamG.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarToplamG.ToString("0,0.00", ci) + "$" + "</td><td>" + tlToplamG.ToString("0,0.00", ci) + "TL" + "</td></tr><tr><td colspan=\"2\">KDV %8</td><td colspan=\"8\" ></td><td>" + euroKDV8.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarKDV8.ToString("0,0.00", ci) + "$" + "</td><td>" + tlKDV8.ToString("0,0.00", ci) + "TL" + "</td></tr><tr><td colspan=\"2\">KDV %18</td><td colspan=\"8\" ></td><td>" + euroKDV18.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarKDV18.ToString("0,0.00", ci) + "$" + "</td><td>" + tlKDV18.ToString("0,0.00", ci) + "TL" + "</td></tr><tr><td colspan=\"2\">KDV'li Genel Toplam</td><td colspan=\"8\" ></td><td>" + euroKDVG.ToString("0,0.00", ci) + "€" + "</td><td>" + dolarKDVG.ToString("0,0.00", ci) + "$" + "</td><td>" + tlKDVG.ToString("0,0.00", ci) + "TL" + "</td></tr></table>");
                sb.Append("<table border = '1'  bgcolor=\"#30e5d3\" style=\"font-family:Arial; font-size:6px;\"><tr><th align=\"center\">" + kurTarih.Date.ToShortDateString() + " tarihindeki TCMB efektif satış döviz kurları baz alınarak hesaplanmıştır.<br>1 Euro/" + (kurEuro).ToString("N04", ci) + " TL, 1 Dolar/" + (kurDolar).ToString("N04", ci) + " TL <br></th></tr></table>");
                sb.Append("<br />");
                sb.Append("<br />");
                alist = 0; satist = 0; kart = 0; gelirt = 0; geliryuzde = 0;
                alistk = 0; satistk = 0; kartk = 0; gelirtk = 0; geliryuzdek = 0;
                tlTutar = 0; tlAlis = 0; euroTutar = 0; dolarTutar = 0; tlToplam = 0;
                dolarToplam = 0; euroToplam = 0; tlToplamK = 0; dolarToplamK = 0;
                euroToplamK = 0; euroToplamG = 0; tlToplamG = 0; euroHizmet = 0;
                tlHizmet = 0; euroHaftasonu = 0; tlHaftasonu = 0; tlKDV8 = 0;
                euroKDV8 = 0; tlKDV18 = 0; euroKDV18 = 0; tlKDVG = 0;
                euroKDVG = 0; hb = 0; dolarHaftasonu = 0; dolarHizmet = 0;
                dolarToplamG = 0; dolarKDV8 = 0; dolarKDV18 = 0; dolarKDVG = 0;
                tlKar = 0; tlGelir = 0; tlAlis = 0;

                StringReader sr = new StringReader(sb.ToString());
                HTMLWorker htmlparser = new HTMLWorker(doc, null, styles);
                var pdfWriter = PdfWriter.GetInstance(doc, stream);
                doc.Open();
                htmlparser.Parse(sr);
                doc.Close();
            }
        }
    }
}
