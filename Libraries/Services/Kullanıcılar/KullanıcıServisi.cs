using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Core;
using Core.Önbellek;
using Core.Data;
using Core.Domain.Genel;
using Core.Domain.Kullanıcılar;
using Data;
using Services.Genel;
using Services.Olaylar;

namespace Services.Kullanıcılar
{
    public partial class KullanıcıServisi : IKullanıcıServisi
    {
        #region Constants
        private const string KULLANICIROLLERI_ALL_KEY = "TS.kullanıcırolü.all-{0}";
        private const string KULLANICIROLLERI_SISTEM_ADI_KEY = "TS.kullanıcırolü.sistemadı-{0}";
        private const string KULLANICIROLLERI_KALIP_KEY = "TS.kullanıcırolü.";

        #endregion

        #region Fields

        private readonly IDepo<Kullanıcı> _kullanıcıDepo;
        private readonly IDepo<KullanıcıŞifre> _kulanıcıŞifreDepo;
        private readonly IDepo<KullanıcıRolü> _kullanıcıRolDepo;
        private readonly IDepo<GenelÖznitelik> _göDepo;
        private readonly IGenelÖznitelikServisi _genelÖznitelikServisi;
        private readonly IDataSağlayıcı _dataSağlayıcı;
        private readonly IDbContext _dbContext;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly KullanıcıAyarları _kullanıcıAyarları;
        private readonly GenelAyarlar _genelAyarlar;

        #endregion

        #region Ctor

        public KullanıcıServisi(IÖnbellekYönetici önbellekYönetici,
            IDepo<Kullanıcı> kullanıcıDepo,
            IDepo<KullanıcıŞifre> kullanıcıŞifreDepo,
            IDepo<KullanıcıRolü> kullanıcıRolDepo,
            IDepo<GenelÖznitelik> göDepo,
            IGenelÖznitelikServisi genelÖznitelikServisi,
            IDataSağlayıcı dataSağlayıcı,
            IDbContext dbContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            KullanıcıAyarları kullanıcıAyarları,
            GenelAyarlar genelAyarlar)
        {
            this._önbellekYönetici = önbellekYönetici;
            this._kullanıcıDepo = kullanıcıDepo;
            this._kulanıcıŞifreDepo = kullanıcıŞifreDepo;
            this._kullanıcıRolDepo = kullanıcıRolDepo;
            this._göDepo = göDepo;
            this._genelÖznitelikServisi = genelÖznitelikServisi;
            this._dataSağlayıcı = dataSağlayıcı;
            this._dbContext = dbContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._kullanıcıAyarları = kullanıcıAyarları;
            this._genelAyarlar = genelAyarlar;
        }

        #endregion

        #region Methods

        #region Kullanıcılar
        public virtual ISayfalıListe<Kullanıcı> TümKullanıcılarıAl(DateTime? oluşturulmaTarihinden = null,
            DateTime? oluşturulmaTarihine = null, int satıcıId = 0,
            int[] kullanıcıRolIdleri = null, string email = null, string kullanıcıadı = null,
            string adı = null, string soyadı = null,
            int doğumTarihi = 0, int doğumAyı = 0,
            string şirket = null, string tel = null, string postaKodu = null,
            string ipAdresi = null,int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _kullanıcıDepo.Tablo;
            if (oluşturulmaTarihinden.HasValue)
                sorgu = sorgu.Where(c => oluşturulmaTarihinden.Value <= c.ŞuTarihdeOluşturuldu);
            if (oluşturulmaTarihine.HasValue)
                sorgu = sorgu.Where(c => oluşturulmaTarihine.Value >= c.ŞuTarihdeOluşturuldu);
            if (satıcıId > 0)
                sorgu = sorgu.Where(c => satıcıId == c.SatıcıId);
            sorgu = sorgu.Where(c => !c.Silindi);
            if (kullanıcıRolIdleri != null && kullanıcıRolIdleri.Length > 0)
                sorgu = sorgu.Where(c => c.KullanıcıRolleri.Select(cr => cr.Id).Intersect(kullanıcıRolIdleri).Any());
            if (!String.IsNullOrWhiteSpace(email))
                sorgu = sorgu.Where(c => c.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(kullanıcıadı))
                sorgu = sorgu.Where(c => c.KullanıcıAdı.Contains(kullanıcıadı));
            if (!String.IsNullOrWhiteSpace(adı))
            {
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.Adı &&
                        z.Attribute.Value.Contains(adı)))
                    .Select(z => z.Kullanıcı);
            }
            if (!String.IsNullOrWhiteSpace(soyadı))
            {
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.Soyadı &&
                        z.Attribute.Value.Contains(soyadı)))
                    .Select(z => z.Kullanıcı);
            }
            //Doğum tarihi, veritabanında dize olarak saklanır.
            //formatı YYYY-MM-DD
            if (doğumTarihi > 0 && doğumAyı > 0)
            {
                string doğumTarihiStr = doğumAyı.ToString("00", CultureInfo.InvariantCulture) + "-" + doğumTarihi.ToString("00", CultureInfo.InvariantCulture);
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.DoğumTarihi &&
                        z.Attribute.Value.Substring(5, 5) == doğumTarihiStr))
                    .Select(z => z.Kullanıcı);
            }
            else if (doğumTarihi > 0)
            {
                string doğumTarihiStr = doğumTarihi.ToString("00", CultureInfo.InvariantCulture);
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.DoğumTarihi &&
                        z.Attribute.Value.Substring(8, 2) == doğumTarihiStr))
                    .Select(z => z.Kullanıcı);
            }
            else if (doğumAyı > 0)
            {
                string doğumTarihiStr = "-" + doğumAyı.ToString("00", CultureInfo.InvariantCulture) + "-";
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.DoğumTarihi &&
                        z.Attribute.Value.Contains(doğumTarihiStr)))
                    .Select(z => z.Kullanıcı);
            }
            //şirket
            if (!String.IsNullOrWhiteSpace(şirket))
            {
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.Şirket &&
                        z.Attribute.Value.Contains(şirket)))
                    .Select(z => z.Kullanıcı);
            }
            //tel
            if (!String.IsNullOrWhiteSpace(tel))
            {
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.Tel &&
                        z.Attribute.Value.Contains(tel)))
                    .Select(z => z.Kullanıcı);
            }
            //postakodu
            if (!String.IsNullOrWhiteSpace(postaKodu))
            {
                sorgu = sorgu
                    .Join(_göDepo.Tablo, x => x.Id, y => y.VarlıkId, (x, y) => new { Kullanıcı = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Kullanıcı" &&
                        z.Attribute.Key == SistemKullanıcıÖznitelikAdları.PostaKodu &&
                        z.Attribute.Value.Contains(postaKodu)))
                    .Select(z => z.Kullanıcı);
            }

            //ip adresi
            if (!String.IsNullOrWhiteSpace(ipAdresi) && GenelYardımcı.GeçerliIpAdresi(ipAdresi))
            {
                sorgu = sorgu.Where(w => w.SonIPAdresi == ipAdresi);
            }
            sorgu = sorgu.OrderByDescending(c => c.ŞuTarihdeOluşturuldu);

            var kullanıcılar = new SayfalıListe<Kullanıcı>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return kullanıcılar;
        }
        public virtual ISayfalıListe<Kullanıcı> OnlineKullanıcılarıAl(DateTime sonİşlemTarihi,
            int[] kullanıcıRolIdleri, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _kullanıcıDepo.Tablo;
            sorgu = sorgu.Where(c => sonİşlemTarihi <= c.SonİşlemTarihi);
            sorgu = sorgu.Where(c => !c.Silindi);
            if (kullanıcıRolIdleri != null && kullanıcıRolIdleri.Length > 0)
                sorgu = sorgu.Where(c => c.KullanıcıRolleri.Select(cr => cr.Id).Intersect(kullanıcıRolIdleri).Any());

            sorgu = sorgu.OrderByDescending(c => c.SonİşlemTarihi);
            var kullanıcılar = new SayfalıListe<Kullanıcı>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return kullanıcılar;
        }
        public virtual void KullanıcıSil(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            if (kullanıcı.SistemHesabı)
                throw new Hata(string.Format("Sistem kullanıcı hesabı ({0}) silinemedi", kullanıcı.SistemAdı));

            kullanıcı.Silindi = true;

            if (_kullanıcıAyarları.SilinenKullanıcılarSonek)
            {
                if (!String.IsNullOrEmpty(kullanıcı.Email))
                    kullanıcı.Email += "-SILINDI";
                if (!String.IsNullOrEmpty(kullanıcı.KullanıcıAdı))
                    kullanıcı.KullanıcıAdı += "-SILINDI";
            }

            KullanıcıGüncelle(kullanıcı);

            //olay Bildirimi
            _olayYayınlayıcı.OlaySilindi(kullanıcı);
        }
        public virtual Kullanıcı KullanıcıAlId(int kullanıcıId)
        {
            if (kullanıcıId == 0)
                return null;

            return _kullanıcıDepo.AlId(kullanıcıId);
        }
        public virtual IList<Kullanıcı> KullanıcıAlIdlerle(int[] kullanıcıIdleri)
        {
            if (kullanıcıIdleri == null || kullanıcıIdleri.Length == 0)
                return new List<Kullanıcı>();

            var sorgu = from c in _kullanıcıDepo.Tablo
                        where kullanıcıIdleri.Contains(c.Id) && !c.Silindi
                        select c;
            var kullanıcılar = sorgu.ToList();
            var sıralananKullanıcılar = new List<Kullanıcı>();
            foreach (int id in kullanıcıIdleri)
            {
                var kullanıcı = kullanıcılar.Find(x => x.Id == id);
                if (kullanıcı != null)
                    sıralananKullanıcılar.Add(kullanıcı);
            }
            return sıralananKullanıcılar;
        }
        public virtual Kullanıcı KullanıcıAlGuid(Guid kullanıcıGuid)
        {
            if (kullanıcıGuid == Guid.Empty)
                return null;

            var sorgu = from c in _kullanıcıDepo.Tablo
                        where c.KullanıcıGuid == kullanıcıGuid
                        orderby c.Id
                        select c;
            var kullanıcı = sorgu.FirstOrDefault();
            return kullanıcı;
        }
        public virtual Kullanıcı KullanıcıAlEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var sorgu = from c in _kullanıcıDepo.Tablo
                        orderby c.Id
                        where c.Email == email
                        select c;
            var kullanıcı = sorgu.FirstOrDefault();
            return kullanıcı;
        }
        public virtual Kullanıcı KullanıcıAlSistemAdı(string sistemAdı)
        {
            if (string.IsNullOrWhiteSpace(sistemAdı))
                return null;

            var sorgu = from c in _kullanıcıDepo.Tablo
                        orderby c.Id
                        where c.SistemAdı == sistemAdı
                        select c;
            var kullanıcı = sorgu.FirstOrDefault();
            return kullanıcı;
        }
        public virtual Kullanıcı KullanıcıAlKullanıcıAdı(string kullanıcıadı)
        {
            if (string.IsNullOrWhiteSpace(kullanıcıadı))
                return null;

            var sorgu = from c in _kullanıcıDepo.Tablo
                        orderby c.Id
                        where c.KullanıcıAdı == kullanıcıadı
                        select c;
            var kullanıcı = sorgu.FirstOrDefault();
            return kullanıcı;
        }
        public virtual Kullanıcı ZiyaretciKullanıcıEkle()
        {
            var kullanıcı = new Kullanıcı
            {
                KullanıcıGuid = Guid.NewGuid(),
                Aktif = true,
                ŞuTarihdeOluşturuldu = DateTime.UtcNow,
                SonİşlemTarihi = DateTime.UtcNow,
            };

            var ziyaretçiRolü = KullanıcıRolüAlSistemAdı(SistemKullanıcıRolAdları.Ziyaretçi);
            if (ziyaretçiRolü == null)
                throw new Hata("'Ziyaretçi' rolü yüklenemedi");
            kullanıcı.KullanıcıRolleri.Add(ziyaretçiRolü);

            _kullanıcıDepo.Ekle(kullanıcı);

            return kullanıcı;
        }
        public virtual void KullanıcıEkle(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            _kullanıcıDepo.Ekle(kullanıcı);

            //olay bildirimi
            _olayYayınlayıcı.OlayEklendi(kullanıcı);
        }
        public virtual void KullanıcıGüncelle(Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            _kullanıcıDepo.Güncelle(kullanıcı);

            //olay bildirimi
            _olayYayınlayıcı.OlayGüncellendi(kullanıcı);
        }
        public virtual void ÖdemeVerileriniSıfırla(Kullanıcı kullanıcı, int siteId,
            bool kuponKodlarınıTemizle = false, bool ödemeÖznitelikleriniTemizle = false,
            bool ödülPuanlarınıTemizle = true,bool ödemeMetodunuTemizle = true)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException();
            
            if (kuponKodlarınıTemizle)
            {
                //_genelÖznitelikServisi.ÖznitelikKaydet<ShippingOption>(kullanıcı, SistemKullanıcıÖznitelikAdları.ku, null);
                //_genelÖznitelikServisi.ÖznitelikKaydet<ShippingOption>(kullanıcı, SistemKullanıcıÖznitelikAdları.GiftCardCouponCodes, null);
            }

            if (ödemeMetodunuTemizle)
            {
                //_genelÖznitelikServisi.ÖznitelikKaydet<ShippingOption>(kullanıcı, SistemKullanıcıÖznitelikAdları.CheckoutAttributes, null, storeId);
            }

            if (ödülPuanlarınıTemizle)
            {
                _genelÖznitelikServisi.ÖznitelikKaydet(kullanıcı, SistemKullanıcıÖznitelikAdları.ÖdemeSırasındaÖdülPuanlarınıKullan, false, siteId);
            }

            if (ödemeMetodunuTemizle)
            {
                _genelÖznitelikServisi.ÖznitelikKaydet<string>(kullanıcı, SistemKullanıcıÖznitelikAdları.SeçiliÖdemeMetodu, null, siteId);
            }
            
            KullanıcıGüncelle(kullanıcı);
        }
        public virtual int ZiyaretciKullanıcıSil(DateTime? oluşturulmaTarihinden, DateTime? oluşturulmaTarihine, bool sepetiDoluOlanlarHariç)
        {
            if (_genelAyarlar.StoredProcedureKullanDestekliyse && _dataSağlayıcı.StoredProceduredDestekli)
            {
                //stored procedure etkin ve veritabanı tarafından destekleniyorsa. 

                #region Stored procedure

                //prepare parameters
                var SepetiDoluOlanlarHariç = _dataSağlayıcı.ParametreAl();
                SepetiDoluOlanlarHariç.ParameterName = "SepetiDoluOlanlarHariç";
                SepetiDoluOlanlarHariç.Value = sepetiDoluOlanlarHariç;
                SepetiDoluOlanlarHariç.DbType = DbType.Boolean;

                var OluşturulmaTarihinden = _dataSağlayıcı.ParametreAl();
                OluşturulmaTarihinden.ParameterName = "CreatedFromUtc";
                OluşturulmaTarihinden.Value = oluşturulmaTarihinden.HasValue ? (object)oluşturulmaTarihinden.Value : DBNull.Value;
                OluşturulmaTarihinden.DbType = DbType.DateTime;

                var OluşturulmaTarihine = _dataSağlayıcı.ParametreAl();
                OluşturulmaTarihine.ParameterName = "CreatedToUtc";
                OluşturulmaTarihine.Value = oluşturulmaTarihine.HasValue ? (object)oluşturulmaTarihine.Value : DBNull.Value;
                OluşturulmaTarihine.DbType = DbType.DateTime;

                var SilinenToplanKayıt = _dataSağlayıcı.ParametreAl();
                SilinenToplanKayıt.ParameterName = "SilinenToplanKayıt";
                SilinenToplanKayıt.Direction = ParameterDirection.Output;
                SilinenToplanKayıt.DbType = DbType.Int32;

                //stored procedure çağır
                _dbContext.SqlKomutunuÇalıştır(
                    "EXEC [ZiyaretçileriSil] @SepetiDoluOlanlarHariç, @OluşturulmaTarihinden, @OluşturulmaTarihine, @SilinenToplanKayıt OUTPUT",
                    false, null,
                    SepetiDoluOlanlarHariç,
                    OluşturulmaTarihinden,
                    OluşturulmaTarihine,
                    SilinenToplanKayıt);

                int silinenToplanKayıt = (SilinenToplanKayıt.Value != DBNull.Value) ? Convert.ToInt32(SilinenToplanKayıt.Value) : 0;
                return silinenToplanKayıt;

                #endregion
            }
            else
            {
                //stored procedure desteklenmiyorsa.LINQ kullanarak

                #region stored procedure yok

                var ziyaretçiRolü = KullanıcıRolüAlSistemAdı(SistemKullanıcıRolAdları.Ziyaretçi);
                if (ziyaretçiRolü == null)
                    throw new Hata("'Ziyaretçi' rolü yüklenemedi");

                var sorgu = _kullanıcıDepo.Tablo;
                if (oluşturulmaTarihinden.HasValue)
                    sorgu = sorgu.Where(c => oluşturulmaTarihinden.Value <= c.ŞuTarihdeOluşturuldu);
                if (oluşturulmaTarihine.HasValue)
                    sorgu = sorgu.Where(c => oluşturulmaTarihine.Value >= c.ŞuTarihdeOluşturuldu);
                sorgu = sorgu.Where(c => c.KullanıcıRolleri.Select(cr => cr.Id).Contains(ziyaretçiRolü.Id));
                /*
                if (sepetiDoluOlanlarHariç)
                    sorgu = sorgu.Where(c => !c.ShoppingCartItems.Any());
                    
                //no orders
                sorgu = from c in sorgu
                        join o in _orderRepository.Tablo on c.Id equals o.CustomerId into c_o
                        from o in c_o.DefaultIfEmpty()
                        where !c_o.Any()
                        select c;
                //no blog comments
                sorgu = from c in sorgu
                        join bc in _blogCommentRepository.Tablo on c.Id equals bc.CustomerId into c_bc
                        from bc in c_bc.DefaultIfEmpty()
                        where !c_bc.Any()
                        select c;
                //no news comments
                sorgu = from c in sorgu
                        join nc in _newsCommentRepository.Tablo on c.Id equals nc.CustomerId into c_nc
                        from nc in c_nc.DefaultIfEmpty()
                        where !c_nc.Any()
                        select c;
                //no product reviews
                sorgu = from c in sorgu
                        join pr in _productReviewRepository.Tablo on c.Id equals pr.CustomerId into c_pr
                        from pr in c_pr.DefaultIfEmpty()
                        where !c_pr.Any()
                        select c;
                //no product reviews helpfulness
                sorgu = from c in sorgu
                        join prh in _productReviewHelpfulnessRepository.Tablo on c.Id equals prh.CustomerId into c_prh
                        from prh in c_prh.DefaultIfEmpty()
                        where !c_prh.Any()
                        select c;
                //no poll voting
                sorgu = from c in sorgu
                        join pvr in _pollVotingRecordRepository.Tablo on c.Id equals pvr.CustomerId into c_pvr
                        from pvr in c_pvr.DefaultIfEmpty()
                        where !c_pvr.Any()
                        select c;
                //no forum posts 
                sorgu = from c in sorgu
                        join fp in _forumPostRepository.Tablo on c.Id equals fp.CustomerId into c_fp
                        from fp in c_fp.DefaultIfEmpty()
                        where !c_fp.Any()
                        select c;
                //no forum topics
                sorgu = from c in sorgu
                        join ft in _forumTopicRepository.Tablo on c.Id equals ft.CustomerId into c_ft
                        from ft in c_ft.DefaultIfEmpty()
                        where !c_ft.Any()
                        select c;
                //don't delete system accounts
                sorgu = sorgu.Where(c => !c.IsSystemAccount);

                //only distinct kullanıcılar (group by ID)
                sorgu = from c in sorgu
                        group c by c.Id
                            into cGroup
                        orderby cGroup.Key
                        select cGroup.FirstOrDefault();
                sorgu = sorgu.OrderBy(c => c.Id);
                */
                var kullanıcılar = sorgu.ToList();


                int silinenToplanKayıt = 0;
                foreach (var c in kullanıcılar)
                {
                    try
                    {
                        //öznitelik sil
                        var attributes = _genelÖznitelikServisi.VarlıkİçinÖznitelikleriAl(c.Id, "Kullanıcı");
                        _genelÖznitelikServisi.ÖznitelikleriSil(attributes);

                        //veritabanından sil
                        _kullanıcıDepo.Sil(c);
                        silinenToplanKayıt++;
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc);
                    }
                }
                return silinenToplanKayıt;

                #endregion
            }
        }

        #endregion

        #region Kullanıcı rolleri
        public virtual void KullanıcıRolüSil(KullanıcıRolü kullanıcıRolü)
        {
            if (kullanıcıRolü == null)
                throw new ArgumentNullException("kullanıcıRolü");

            if (kullanıcıRolü.SistemRolü)
                throw new Hata("Sistem rolü silinemedi");

            _kullanıcıRolDepo.Sil(kullanıcıRolü);

            _önbellekYönetici.KalıpİleSil(KULLANICIROLLERI_KALIP_KEY);

            //olay bildirimi
            _olayYayınlayıcı.OlaySilindi(kullanıcıRolü);
        }
        public virtual KullanıcıRolü KullanıcıRolüAlId(int kullanıcıRolId)
        {
            if (kullanıcıRolId == 0)
                return null;

            return _kullanıcıRolDepo.AlId(kullanıcıRolId);
        }
        public virtual KullanıcıRolü KullanıcıRolüAlSistemAdı(string sistemAdı)
        {
            if (String.IsNullOrWhiteSpace(sistemAdı))
                return null;

            string key = string.Format(KULLANICIROLLERI_SISTEM_ADI_KEY, sistemAdı);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = from cr in _kullanıcıRolDepo.Tablo
                            orderby cr.Id
                            where cr.SistemAdı == sistemAdı
                            select cr;
                var kullanıcıRolü = sorgu.FirstOrDefault();
                return kullanıcıRolü;
            });
        }
        public virtual IList<KullanıcıRolü> TümKullanıcıRolleriniAl(bool gizliGöster = false)
        {
            string key = string.Format(KULLANICIROLLERI_ALL_KEY, gizliGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = from cr in _kullanıcıRolDepo.Tablo
                            orderby cr.Adı
                            where gizliGöster || cr.Aktif
                            select cr;
                var kullanıcıRolleri = sorgu.ToList();
                return kullanıcıRolleri;
            });
        }
        public virtual void KullanıcıRolüEkle(KullanıcıRolü kullanıcıRolü)
        {
            if (kullanıcıRolü == null)
                throw new ArgumentNullException("kulanıcıRolü");

            _kullanıcıRolDepo.Ekle(kullanıcıRolü);

            _önbellekYönetici.KalıpİleSil(KULLANICIROLLERI_KALIP_KEY);

            //olay bildirimi
            _olayYayınlayıcı.OlayEklendi(kullanıcıRolü);
        }
        public virtual void KullanıcıRolüGüncelle(KullanıcıRolü kulanıcıRolü)
        {
            if (kulanıcıRolü == null)
                throw new ArgumentNullException("kulanıcıRolü");

            _kullanıcıRolDepo.Güncelle(kulanıcıRolü);

            _önbellekYönetici.KalıpİleSil(KULLANICIROLLERI_KALIP_KEY);

            //olay bildirimi
            _olayYayınlayıcı.OlayGüncellendi(kulanıcıRolü);
        }

        #endregion

        #region Kullanıcı passwords

        public virtual IList<KullanıcıŞifre> KullanıcıŞifreleriAl(int? kullanıcıId = null,
            ŞifreFormatı? şifreFormatı = null, int? geriDönenŞifreler = null)
        {
            var sorgu = _kulanıcıŞifreDepo.Tablo;

            //kullanıcıId ile filtrele 
            if (kullanıcıId.HasValue)
                sorgu = sorgu.Where(password => password.KullanıcıId == kullanıcıId.Value);

            //şifreFormatı ile filtrele 
            if (şifreFormatı.HasValue)
                sorgu = sorgu.Where(password => password.ŞifreFormatId == (int)(şifreFormatı.Value));

            //son şifreyi al
            if (geriDönenŞifreler.HasValue)
                sorgu = sorgu.OrderByDescending(password => password.OluşturulmaTarihi).Take(geriDönenŞifreler.Value);

            return sorgu.ToList();
        }

        public virtual KullanıcıŞifre MevcutŞifreAl(int kulanıcıId)
        {
            if (kulanıcıId == 0)
                return null;

            //son şifreyi dondür
            return KullanıcıŞifreleriAl(kulanıcıId, geriDönenŞifreler: 1).FirstOrDefault();
        }


        public virtual void KullanıcıŞifresiEkle(KullanıcıŞifre kullanıcıŞifresi)
        {
            if (kullanıcıŞifresi == null)
                throw new ArgumentNullException("kullanıcıŞifresi");

            _kulanıcıŞifreDepo.Ekle(kullanıcıŞifresi);

            //olay bildirimi
            _olayYayınlayıcı.OlayEklendi(kullanıcıŞifresi);
        }


        public virtual void KullanıcıŞifresiGüncelle(KullanıcıŞifre kullanıcıŞifresi)
        {
            if (kullanıcıŞifresi == null)
                throw new ArgumentNullException("kullanıcıŞifresi");

            _kulanıcıŞifreDepo.Güncelle(kullanıcıŞifresi);

            //olay bildirimi
            _olayYayınlayıcı.OlayGüncellendi(kullanıcıŞifresi);
        }

        #endregion

        #endregion
    }
}
