using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using ImageResizer;
using Core;
using Core.Data;
using Core.Domain.Medya;
using Data;
using Services.Yapılandırma;
using Services.Olaylar;
using Services.Logging;
using Services.Seo;
using Microsoft.AspNetCore.Hosting;

namespace Services.Medya
{
    public partial class ResimServisi : IResimServisi
    {
        #region Const

        private const int COKLU_THUMB_KLASORLERI_UZUNLUGU = 3;

        #endregion

        #region Fields

        private readonly IDepo<Resim> _resimDepo;
        private readonly IAyarlarServisi _ayarlarServisi;
        private readonly IWebYardımcısı _webYardımcısı;
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcısı;
        private readonly MedyaAyarları _medyaAyarları;
        private readonly IDataSağlayıcı _dataSağlayıcı;
        private readonly IHostingEnvironment _hostingEnvironment;

        #endregion

        #region Ctor
        public ResimServisi(IDepo<Resim> pictureRepository,
            IAyarlarServisi ayarlarServisi,
            IWebYardımcısı webYardımcısı,
            ILogger logger,
            IDbContext dbContext,
            IOlayYayınlayıcı olayYayınlayıcısı,
            MedyaAyarları medyaAyarları,
            IDataSağlayıcı dataSağlayıcı,
            IHostingEnvironment hostingEnvironment)
        {
            this._resimDepo = pictureRepository;
            this._ayarlarServisi = ayarlarServisi;
            this._webYardımcısı = webYardımcısı;
            this._logger = logger;
            this._dbContext = dbContext;
            this._olayYayınlayıcısı = olayYayınlayıcısı;
            this._medyaAyarları = medyaAyarları;
            this._dataSağlayıcı = dataSağlayıcı;
            this._hostingEnvironment = hostingEnvironment;
        }

        #endregion

        #region Utilities
        protected virtual Size BoyutlarıHesapla(Size orijinalBoyut, int hedefBoyut,
            YeniBoyutTipi yeniBoyutTipi = YeniBoyutTipi.EnUzunYan, bool BoyutunPozitifOlduğunuDoğrula = true)
        {
            float genişlik, yükseklik;

            switch (yeniBoyutTipi)
            {
                case YeniBoyutTipi.EnUzunYan:
                    if (orijinalBoyut.Height > orijinalBoyut.Width)
                    {
                        // portre
                        genişlik = orijinalBoyut.Width * (hedefBoyut / (float)orijinalBoyut.Height);
                        yükseklik = hedefBoyut;
                    }
                    else
                    {
                        // landscape or square
                        genişlik = hedefBoyut;
                        yükseklik = orijinalBoyut.Height * (hedefBoyut / (float)orijinalBoyut.Width);
                    }
                    break;
                case YeniBoyutTipi.Genişlik:
                    genişlik = hedefBoyut;
                    yükseklik = orijinalBoyut.Height * (hedefBoyut / (float)orijinalBoyut.Width);
                    break;
                case YeniBoyutTipi.Yükseklik:
                    genişlik = orijinalBoyut.Width * (hedefBoyut / (float)orijinalBoyut.Height);
                    yükseklik = hedefBoyut;
                    break;
                default:
                    throw new Exception("Not supported YeniBoyutTipi");
            }

            if (BoyutunPozitifOlduğunuDoğrula)
            {
                if (genişlik < 1)
                    genişlik = 1;
                if (yükseklik < 1)
                    yükseklik = 1;
            }

            return new Size((int)Math.Round(genişlik), (int)Math.Round(yükseklik));
        }
        protected virtual string MimeTipindenDosyaUzantısınıAl(string mimeTipi)
        {
            if (mimeTipi == null)
                return null;

            string[] parçalar = mimeTipi.Split('/');
            string sonParça = parçalar[parçalar.Length - 1];
            switch (sonParça)
            {
                case "pjpeg":
                    sonParça = "jpg";
                    break;
                case "x-png":
                    sonParça = "png";
                    break;
                case "x-icon":
                    sonParça = "ico";
                    break;
            }
            return sonParça;
        }
        protected virtual byte[] DosyadanResimYükle(int resimId, string mimeTipi)
        {
            string sonParça = MimeTipindenDosyaUzantısınıAl(mimeTipi);
            string dosyaAdı = string.Format("{0}_0.{1}", resimId.ToString("0000000"), sonParça);
            var dosyaYolu = ResimLocalYolunuAl(dosyaAdı);
            if (!File.Exists(dosyaYolu))
                return new byte[0];
            return File.ReadAllBytes(dosyaYolu);
        }
        protected virtual void DosyayaResimKaydet(int resimId, byte[] resimBinary, string mimeTipi)
        {
            string sonParça = MimeTipindenDosyaUzantısınıAl(mimeTipi);
            string dosyaAdı = string.Format("{0}_0.{1}", resimId.ToString("0000000"), sonParça);
            File.WriteAllBytes(ResimLocalYolunuAl(dosyaAdı), resimBinary);
        }
        protected virtual void DosyaSistemindenResimSil(Resim resim)
        {
            if (resim == null)
                throw new ArgumentNullException("resim");

            string sonParça = MimeTipindenDosyaUzantısınıAl(resim.MimeTipi);
            string dosyaAdı = string.Format("{0}_0.{1}", resim.Id.ToString("0000000"), sonParça);
            string dosyaYolu = ResimLocalYolunuAl(dosyaAdı);
            if (File.Exists(dosyaYolu))
            {
                File.Delete(dosyaYolu);
            }
        }
        protected virtual void ResimThumbSil(Resim resim)
        {
            string filtre = string.Format("{0}*.*", resim.Id.ToString("0000000"));
            var thumbKlasörYolu = GenelYardımcı.MapPath("~/content/images/thumbs");
            string[] mevcutDosyalar = Directory.GetFiles(thumbKlasörYolu, filtre, SearchOption.AllDirectories);
            foreach (string mevcutDosyaAdı in mevcutDosyalar)
            {
                var thumbDosyaYolu = ThumbYoluAl(mevcutDosyaAdı);
                File.Delete(thumbDosyaYolu);
            }
        }
        protected virtual string ThumbYoluAl(string thumbDosyaAdı)
        {
            var thumbKlasörYolu = GenelYardımcı.MapPath("~/content/images/thumbs");
            if (_medyaAyarları.CokluThumbKlasorleri)
            {
                //dosya adından ilk iki harfi al
                var UzantısızDosyaAdı = Path.GetFileNameWithoutExtension(thumbDosyaAdı);
                if (UzantısızDosyaAdı != null && UzantısızDosyaAdı.Length > COKLU_THUMB_KLASORLERI_UZUNLUGU)
                {
                    var altKlasörAdı = UzantısızDosyaAdı.Substring(0, COKLU_THUMB_KLASORLERI_UZUNLUGU);
                    thumbKlasörYolu = Path.Combine(thumbKlasörYolu, altKlasörAdı);
                    if (!Directory.Exists(thumbKlasörYolu))
                    {
                        Directory.CreateDirectory(thumbKlasörYolu);
                    }
                }
            }
            var thumbDosyaYolu = Path.Combine(thumbKlasörYolu, thumbDosyaAdı);
            return thumbDosyaYolu;
        }
        protected virtual string ThumbUrlAl(string thumbDosyaAdı, string kaynakKonumu = null)
        {
            kaynakKonumu = !String.IsNullOrEmpty(kaynakKonumu)
                                    ? kaynakKonumu
                                    : _webYardımcısı.SiteKonumuAl();
            var url = kaynakKonumu + "content/images/thumbs/";

            if (_medyaAyarları.CokluThumbKlasorleri)
            {
                //dosya adından ilk iki harfi al
                var uzantısızDosyaAdı = Path.GetFileNameWithoutExtension(thumbDosyaAdı);
                if (uzantısızDosyaAdı != null && uzantısızDosyaAdı.Length > COKLU_THUMB_KLASORLERI_UZUNLUGU)
                {
                    var altKlasörAdı = uzantısızDosyaAdı.Substring(0, COKLU_THUMB_KLASORLERI_UZUNLUGU);
                    url = url + altKlasörAdı + "/";
                }
            }

            url = url + thumbDosyaAdı;
            return url;
        }
        protected virtual string ResimLocalYolunuAl(string dosyaAdı)
        {
            return Path.Combine(GenelYardımcı.MapPath("~/content/images/"), dosyaAdı);
        }
        protected virtual byte[] ResimBinaryYükle(Resim resim, bool dbDen)
        {
            if (resim == null)
                throw new ArgumentNullException("resim");

            var sonuç = dbDen
                ? resim.ResimBinary
                : DosyadanResimYükle(resim.Id, resim.MimeTipi);
            return sonuç;
        }
        protected virtual bool ThumbZatenMevcut(string thumbDosyaYolu, string thumbDosyaAdı)
        {
            return File.Exists(thumbDosyaYolu);
        }
        protected virtual void ThumbKaydet(string thumbDosyaYolu, string thumbDosyaAdı, string mimeTipi, byte[] binary)
        {
            File.WriteAllBytes(thumbDosyaYolu, binary);
        }

        #endregion

        #region Resim yerel yol / URL yöntemlerini alma

        public virtual byte[] ResimBinaryYükle(Resim resim)
        {
            return ResimBinaryYükle(resim, this.VeritabanındaDepola);
        }
        public virtual string ResimSeAdıAl(string name)
        {
            return name;//SEO ayarlarını ekle
        }
        public virtual string ResimVarsayılanUrlAl(int hedefBüyüklüğü = 0,
            ResimTipi varsayılanResimTipi = ResimTipi.Varlık,
            string siteKonumu = null)
        {
            string varsayılanResimDosyaAdı;
            switch (varsayılanResimTipi)
            {
                case ResimTipi.Avatar:
                    varsayılanResimDosyaAdı = _ayarlarServisi.AyarAlKey("Medya.Kullanıcı.VarsayılanAvatarResmi", "default-avatar.jpg");
                    break;
                case ResimTipi.Varlık:
                default:
                    varsayılanResimDosyaAdı = _ayarlarServisi.AyarAlKey("Medya.VarsayılanResim", "default-image.png");
                    break;
            }
            string dosyaYolu = ResimLocalYolunuAl(varsayılanResimDosyaAdı);
            if (!File.Exists(dosyaYolu))
            {
                return "";
            }


            if (hedefBüyüklüğü == 0)
            {
                string url = (!String.IsNullOrEmpty(siteKonumu)
                                 ? siteKonumu
                                 : _webYardımcısı.SiteKonumuAl())
                                 + "content/images/" + varsayılanResimDosyaAdı;
                return url;
            }
            else
            {
                string dosyaUzantısı = Path.GetExtension(dosyaYolu);
                string thumbDosyaAdı = string.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(dosyaYolu),
                    hedefBüyüklüğü,
                    dosyaUzantısı);
                var thumbDosyaYolu = ThumbYoluAl(thumbDosyaAdı);
                if (!ThumbZatenMevcut(thumbDosyaYolu, thumbDosyaAdı))
                {
                    using (var b = new Bitmap(dosyaYolu))
                    {
                        using (var destStream = new MemoryStream())
                        {
                            var yeniBoyut = BoyutlarıHesapla(b.Size, hedefBüyüklüğü);
                            ImageBuilder.Current.Build(b, destStream, new ResizeSettings
                            {
                                Width = yeniBoyut.Width,
                                Height = yeniBoyut.Height,
                                Scale = ScaleMode.Both,
                                Quality = _medyaAyarları.VarsayılanResimKalitesi
                            });
                            var destBinary = destStream.ToArray();
                            ThumbKaydet(thumbDosyaYolu, thumbDosyaAdı, "", destBinary);
                        }
                    }
                }
                var url = ThumbUrlAl(thumbDosyaAdı, siteKonumu);
                return url;
            }
        }
        public virtual string ResimUrlAl(int resimId,
            int hedefBüyüklüğü = 0,
            bool varsayılanResimGöster = true,
            string kaynakKonumu = null,
            ResimTipi varsayılanResimTipi = ResimTipi.Varlık)
        {
            var resim = ResimAlId(resimId);
            return ResimUrlAl(resim, hedefBüyüklüğü, varsayılanResimGöster, kaynakKonumu, varsayılanResimTipi);
        }
        public virtual string ResimUrlAl(Resim resim,
            int hedefBüyüklüğü = 0,
            bool varsayılanResimGöster = true,
            string kaynakKonumu = null,
            ResimTipi varsayılanResimTipi = ResimTipi.Varlık)
        {
            string url = string.Empty;
            byte[] resimBinary = null;
            if (resim != null)
                resimBinary = ResimBinaryYükle(resim);
            if (resim == null || resimBinary == null || resimBinary.Length == 0)
            {
                if (varsayılanResimGöster)
                {
                    url = ResimVarsayılanUrlAl(hedefBüyüklüğü, varsayılanResimTipi, kaynakKonumu);
                }
                return url;
            }

            if (resim.Yeni)
            {
                ResimThumbSil(resim);

                resim = ResimGüncelle(resim.Id,
                    resimBinary,
                    resim.MimeTipi,
                    resim.SeoDosyaAdı,
                    resim.AltÖznitelik,
                    resim.BaşlıkÖznitelik,
                    false,
                    false);
            }

            var seoFileName = resim.SeoDosyaAdı; 

            string sonParça = MimeTipindenDosyaUzantısınıAl(resim.MimeTipi);
            string thumbDosyaAdı;
            if (hedefBüyüklüğü == 0)
            {
                thumbDosyaAdı = !String.IsNullOrEmpty(seoFileName)
                    ? string.Format("{0}_{1}.{2}", resim.Id.ToString("0000000"), seoFileName, sonParça)
                    : string.Format("{0}.{1}", resim.Id.ToString("0000000"), sonParça);
            }
            else
            {
                thumbDosyaAdı = !String.IsNullOrEmpty(seoFileName)
                    ? string.Format("{0}_{1}_{2}.{3}", resim.Id.ToString("0000000"), seoFileName, hedefBüyüklüğü, sonParça)
                    : string.Format("{0}_{1}.{2}", resim.Id.ToString("0000000"), hedefBüyüklüğü, sonParça);
            }
            string thumbDosyaYolu = ThumbYoluAl(thumbDosyaAdı);

            // adı verilen mutex aynı dosyaların farklı iş parçacıklarında oluşturulmasını önlemeye yardımcı olur,
            // ve kod belirli bir dosya için engellendiği için performansı önemli ölçüde düşürmez.
            using (var mutex = new Mutex(false, thumbDosyaAdı))
            {
                if (!ThumbZatenMevcut(thumbDosyaYolu, thumbDosyaAdı))
                {
                    mutex.WaitOne();

                    //dosyanın zaten oluşturulduğunu kontrol eder
                    if (!ThumbZatenMevcut(thumbDosyaYolu, thumbDosyaAdı))
                    {
                        byte[] resimBinaryYenidenBoyutlandırıldı;

                        //yeniden boyutlandırma gerekli
                        if (hedefBüyüklüğü != 0)
                        {
                            using (var stream = new MemoryStream(resimBinary))
                            {
                                Bitmap b = null;
                                try
                                {
                                    b = new Bitmap(stream);
                                }
                                catch (ArgumentException exc)
                                {
                                    _logger.Hata(string.Format("Resim thumb oluşturulurken hata oluştu. ID={0}", resim.Id),
                                        exc);
                                }

                                if (b == null)
                                {
                                    //bitmap bazı sebeplerden ötürü yüklenemedi
                                    return url;
                                }

                                using (var destStream = new MemoryStream())
                                {
                                    var yeniBoyut = BoyutlarıHesapla(b.Size, hedefBüyüklüğü);
                                    ImageBuilder.Current.Build(b, destStream, new ResizeSettings
                                    {
                                        Width = yeniBoyut.Width,
                                        Height = yeniBoyut.Height,
                                        Scale = ScaleMode.Both,
                                        Quality = _medyaAyarları.VarsayılanResimKalitesi
                                    });
                                    resimBinaryYenidenBoyutlandırıldı = destStream.ToArray();
                                    b.Dispose();
                                }
                            }
                        }
                        else
                        {
                            //resimBinary kopyasını oluştur
                            resimBinaryYenidenBoyutlandırıldı = resimBinary.ToArray();
                        }

                        ThumbKaydet(thumbDosyaYolu, thumbDosyaAdı, resim.MimeTipi, resimBinaryYenidenBoyutlandırıldı);
                    }

                    mutex.ReleaseMutex();
                }

            }
            url = ThumbUrlAl(thumbDosyaAdı, kaynakKonumu);
            return url;
        }
        public virtual string ThumbYoluAl(Resim resim, int hedefBüyüklüğü = 0, bool varsayılanResimGöster = true)
        {
            string url = ResimUrlAl(resim, hedefBüyüklüğü, varsayılanResimGöster);
            if (String.IsNullOrEmpty(url))
                return String.Empty;

            return ThumbYoluAl(Path.GetFileName(url));
        }

        #endregion

        #region CRUD methods
        
        public virtual Resim ResimAlId(int resimId)
        {
            if (resimId == 0)
                return null;

            return _resimDepo.AlId(resimId);
        }
        public virtual void ResimSil(Resim resim)
        {
            if (resim == null)
                throw new ArgumentNullException("resim");

            //thumb sil
            ResimThumbSil(resim);

            //dosya sisteminden sil
            if (!this.VeritabanındaDepola)
                DosyaSistemindenResimSil(resim);

            //veritabanından sil
            _resimDepo.Sil(resim);

            //olay bildirimi
            _olayYayınlayıcısı.OlaySilindi(resim);
        }
        public virtual ISayfalıListe<Resim> ResimleriAl(int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = from p in _resimDepo.Tablo
                        orderby p.Id descending
                        select p;
            var resimler = new SayfalıListe<Resim>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return resimler;
        }
        public virtual Resim ResimEkle(byte[] resimBinary, string mimeTipi, string seoDosyaAdı,
            string altÖznitelik = null, string başlıkÖznitelik = null,
            bool Yeni = true, bool BinaryDoğrula = true)
        {
            mimeTipi = GenelYardımcı.BoşKontrol(mimeTipi);
            mimeTipi = GenelYardımcı.MaksimumUzunlukKontrol(mimeTipi, 20);

            seoDosyaAdı = GenelYardımcı.MaksimumUzunlukKontrol(seoDosyaAdı, 100);

            if (BinaryDoğrula)
                resimBinary = ResimDoğrula(resimBinary, mimeTipi);

            var resim = new Resim
            {
                ResimBinary = this.VeritabanındaDepola ? resimBinary : new byte[0],
                MimeTipi = mimeTipi,
                SeoDosyaAdı = seoDosyaAdı,
                AltÖznitelik = altÖznitelik,
                BaşlıkÖznitelik = başlıkÖznitelik,
                Yeni = Yeni,
            };
            _resimDepo.Ekle(resim);

            if (!this.VeritabanındaDepola)
                DosyayaResimKaydet(resim.Id, resimBinary, mimeTipi);

            //olay bildirimi
            _olayYayınlayıcısı.OlayEklendi(resim);

            return resim;
        }
        public virtual Resim ResimGüncelle(int resimId, byte[] resimBinary, string mimeTipi,
            string seoDosyaAdı, string altÖznitelik = null, string başlıkÖznitelik = null,
            bool Yeni = true, bool BinaryDoğrula = true)
        {
            mimeTipi = GenelYardımcı.BoşKontrol(mimeTipi);
            mimeTipi = GenelYardımcı.MaksimumUzunlukKontrol(mimeTipi, 20);

            seoDosyaAdı = GenelYardımcı.MaksimumUzunlukKontrol(seoDosyaAdı, 100);

            if (BinaryDoğrula)
                resimBinary = ResimDoğrula(resimBinary, mimeTipi);

            var resim = ResimAlId(resimId);
            if (resim == null)
                return null;

            //Bir resim değiştirildiyse eski thumb sil
            if (seoDosyaAdı != resim.SeoDosyaAdı)
                ResimThumbSil(resim);

            resim.ResimBinary = this.VeritabanındaDepola ? resimBinary : new byte[0];
            resim.MimeTipi = mimeTipi;
            resim.SeoDosyaAdı = seoDosyaAdı;
            resim.AltÖznitelik = altÖznitelik;
            resim.BaşlıkÖznitelik = başlıkÖznitelik;
            resim.Yeni = Yeni;

            _resimDepo.Güncelle(resim);

            if (!this.VeritabanındaDepola)
                DosyayaResimKaydet(resim.Id, resimBinary, mimeTipi);

            //olay bildirimi
            _olayYayınlayıcısı.OlayGüncellendi(resim);

            return resim;
        }
        public virtual Resim SeoDosyaAdıAyarla(int resimId, string seoDosyaAdı)
        {
            var resim = ResimAlId(resimId);
            if (resim == null)
                throw new ArgumentException("Belirtilen Idli resim bulunamadı");

            //update if it has been changed
            if (seoDosyaAdı != resim.SeoDosyaAdı)
            {
                //update resim
                resim = ResimGüncelle(resim.Id,
                    ResimBinaryYükle(resim),
                    resim.MimeTipi,
                    seoDosyaAdı,
                    resim.AltÖznitelik,
                    resim.BaşlıkÖznitelik,
                    true,
                    false);
            }
            return resim;
        }
        public virtual byte[] ResimDoğrula(byte[] resimBinary, string mimeTipi)
        {
            using (var destStream = new MemoryStream())
            {
                ImageBuilder.Current.Build(resimBinary, destStream, new ResizeSettings
                {
                    MaxWidth = _medyaAyarları.MaksimumResimBoyutu,
                    MaxHeight = _medyaAyarları.MaksimumResimBoyutu,
                    Quality = _medyaAyarları.VarsayılanResimKalitesi
                });
                return destStream.ToArray();
            }
        }
        private class HashItem : IComparable, IComparable<HashItem>
        {
            public int ResimId { get; set; }
            public byte[] Hash { get; set; }

            public int CompareTo(object obj)
            {
                return CompareTo(obj as HashItem);
            }

            public int CompareTo(HashItem other)
            {
                return other == null ? -1 : ResimId.CompareTo(other.ResimId);
            }
        }
        public IDictionary<int, string> ResimHashAl(int[] picturesIds)
        {
            var desteklenenBinaryHashUzunluğu = _dataSağlayıcı.DesteklenenBinaryHashUzunluğu();
            if (desteklenenBinaryHashUzunluğu == 0 || !picturesIds.Any())
                return new Dictionary<int, string>();

            const string strKomutu = "SELECT [Id] as [ResimId], HASHBYTES('sha1', substring([ResimBinary], 0, {0})) as [Hash] FROM [Resim] where id in ({1})";
            return _dbContext.SqlSorgusu<HashItem>(String.Format(strKomutu, desteklenenBinaryHashUzunluğu, picturesIds.Select(p => p.ToString()).Aggregate((all, current) => all + ", " + current))).Distinct()
                .ToDictionary(p => p.ResimId, p => BitConverter.ToString(p.Hash).Replace("-", ""));
        }

        #endregion

        #region Properties
        public virtual bool VeritabanındaDepola
        {
            get
            {
                return _ayarlarServisi.AyarAlKey("Medya.Resim.VeritabanındaDepola", true);
            }
            set
            {
                //yeni bir değer olup olmadığını kontrol et
                if (this.VeritabanındaDepola == value)
                    return;

                //yeni ayar değerini kaydet
                _ayarlarServisi.AyarAyarla("Medya.Resim.VeritabanındaDepola", value);

                int sayfaIndeksi = 0;
                const int sayfaBüyüklüğü = 400;
                var orijinalProxyOluşturmaEtkin = _dbContext.ProxyOluşturmaEtkin;
                try
                {
                   
                    _dbContext.ProxyOluşturmaEtkin = false;

                    while (true)
                    {
                        var resimler = this.ResimleriAl(sayfaIndeksi, sayfaBüyüklüğü);
                        sayfaIndeksi++;

                        //tüm resimler dönüştürüldü mü?
                        if (!resimler.Any())
                            break;

                        foreach (var resim in resimler)
                        {
                            var resimBinary = ResimBinaryYükle(resim, !value);
                            if (value)
                                //dosya sisteminden sil şimdi veritabanında
                                DosyaSistemindenResimSil(resim);
                            else
                                //şimdi dosya sisteminde
                                DosyayaResimKaydet(resim.Id, resimBinary, resim.MimeTipi);
                            //uygun özellikleri güncelle
                            resim.ResimBinary = value ? resimBinary : new byte[0];
                            resim.Yeni = true;
                        }
                        //Bir kerede tümünü kurtar
                        _resimDepo.Güncelle(resimler);
                        foreach (var resim in resimler)
                        {
                            _dbContext.Ayır(resim);
                        }
                    }
                }
                finally
                {
                    _dbContext.ProxyOluşturmaEtkin = orijinalProxyOluşturmaEtkin;
                }
            }
        }

        #endregion
    }
}
