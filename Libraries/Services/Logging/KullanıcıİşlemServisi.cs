using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Domain.Kullanıcılar;
using Core.Domain.Logging;
using Core.Data;
using Core.Önbellek;
using Data;
using Core.Domain.Genel;

namespace Services.Logging
{
    public partial class KullanıcıİşlemServisi : IKullanıcıİşlemServisi
    {
        private const string ISLEMTIPI_ALL_KEY = "işlemTipi.all";
        private const string ISLEMTIPI_PATTERN_KEY = "işlemTipi.";
        private readonly IDepo<İşlemTipi> _işlemTipiDepo;
        private readonly IDepo<İşlem> _işlemDepo;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IWebYardımcısı _webYardımcısı;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IDataSağlayıcı _dataSağlayıcı;
        private readonly GenelAyarlar _genelAyarlar;
        public KullanıcıİşlemServisi(IDepo<İşlemTipi> işlemTipiDepo,
            IDepo<İşlem> işlemDepo,
            IÖnbellekYönetici önbellekYönetici,
            IWebYardımcısı webYardımcısı,
            IWorkContext workContext,
            IDbContext dbContext,
            IDataSağlayıcı dataSağlayıcı,
            GenelAyarlar genelAyarlar)
        {
            this._işlemTipiDepo = işlemTipiDepo;
            this._işlemDepo = işlemDepo;
            this._önbellekYönetici = önbellekYönetici;
            this._webYardımcısı = webYardımcısı;
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._dataSağlayıcı = dataSağlayıcı;
            this._genelAyarlar = genelAyarlar;
        }
        [Serializable]
        public class ÖnbellekİçinİşlemTipi
        {
            public int Id { get; set; }
            public string SistemAnahtarKelimeleri { get; set; }
            public string Adı { get; set; }
            public bool Etkin { get; set; }
        }
        public virtual ISayfalıListe<İşlem> TümİşlemleriAl(DateTime? şuTarihden = default(DateTime?), DateTime? şuTarihe = default(DateTime?), int? kullanıcıId = default(int?), int işlemTipiId = 0, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, string ipAdresi = null)
        {
            var sorgu = _işlemDepo.Tablo;
            if (!String.IsNullOrEmpty(ipAdresi))
                sorgu = sorgu.Where(al => al.IpAdresi.Contains(ipAdresi));
            if (şuTarihden.HasValue)
                sorgu = sorgu.Where(al => şuTarihden.Value <= al.OluşturulmaTarihi);
            if (şuTarihe.HasValue)
                sorgu = sorgu.Where(al => şuTarihe.Value >= al.OluşturulmaTarihi);
            if (işlemTipiId > 0)
                sorgu = sorgu.Where(al => işlemTipiId == al.İşlemTipiId);
            if (kullanıcıId.HasValue)
                sorgu = sorgu.Where(al => kullanıcıId.Value == al.KullanıcıId);

            sorgu = sorgu.OrderByDescending(al => al.OluşturulmaTarihi);

            var işem = new SayfalıListe<İşlem>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return işem;
        }

        public virtual void TümİşlemleriTemizle()
        {
            if (_genelAyarlar.StoredProcedureKullanDestekliyse && _dataSağlayıcı.StoredProceduredDestekli)
            {
                string işlemTabloAdı = _dbContext.GetTableName<İşlem>();
                _dbContext.SqlKomutunuÇalıştır(String.Format("TRUNCATE TABLE [{0}]", işlemTabloAdı));
            }
            else
            {
                var işlem = _işlemDepo.Tablo.ToList();
                foreach (var işlemÖğesi in işlem)
                    _işlemDepo.Sil(işlemÖğesi);
            }
        }

        public virtual IList<İşlemTipi> TümİşlemTipleriAl()
        {
            var sorgu = from alt in _işlemTipiDepo.Tablo
                       orderby alt.Adı
                       select alt;
            var işlemTipleri = sorgu.ToList();
            return işlemTipleri;
        }

        public virtual İşlem İşlemAlId(int işlemTipiId)
        {
            if (işlemTipiId == 0)
                return null;

            return _işlemDepo.AlId(işlemTipiId);
        }

        public virtual İşlem İşlemEkle(string sistemAnahtarKelimleri, string yorum, params object[] yorumDeğerleri)
        {
            return İşlemEkle(_workContext.MevcutKullanıcı, sistemAnahtarKelimleri, yorum, yorumDeğerleri);
        }

        public virtual İşlem İşlemEkle(Kullanıcı kullanıcı, string sistemAnahtarKelimleri, string yorum, params object[] yorumDeğerleri)
        {
            if (kullanıcı == null)
                return null;

            var işlemTipleri = TümÖnbelleklenenİşlemTipleriAl();
            var işlemTipi = işlemTipleri.ToList().Find(at => at.SistemAnahtarKelimeleri == sistemAnahtarKelimleri);
            if (işlemTipi == null || !işlemTipi.Etkin)
                return null;

            yorum = GenelYardımcı.BoşKontrol(yorum);
            yorum = string.Format(yorum, yorumDeğerleri);
            yorum = GenelYardımcı.MaksimumUzunlukKontrol(yorum, 4000);



            var işlem = new İşlem();
            işlem.İşlemTipiId = işlemTipi.Id;
            işlem.Kullanıcı = kullanıcı;
            işlem.Yorum = yorum;
            işlem.OluşturulmaTarihi = DateTime.UtcNow;
            işlem.IpAdresi = _webYardımcısı.MevcutIpAdresiAl();

            _işlemDepo.Ekle(işlem);

            return işlem;
        }

        public virtual IList<ÖnbellekİçinİşlemTipi> TümÖnbelleklenenİşlemTipleriAl()
        {
            string key = string.Format(ISLEMTIPI_ALL_KEY);
            return _önbellekYönetici.Al(key, () =>
            {
                var sonuç = new List<ÖnbellekİçinİşlemTipi>();
                var işlemTipleri = TümİşlemTipleriAl();
                foreach (var alt in işlemTipleri)
                {
                    var önbellekİçinİşlemTipi = new ÖnbellekİçinİşlemTipi
                    {
                        Id = alt.Id,
                        SistemAnahtarKelimeleri = alt.SistemAnahtarKelimeleri,
                        Adı = alt.Adı,
                        Etkin = alt.Etkin
                    };
                    sonuç.Add(önbellekİçinİşlemTipi);
                }
                return sonuç;
            });
        }

        public virtual void İşlemSil(İşlem işlem)
        {
            if (işlem == null)
                throw new ArgumentNullException("işlem");
            _işlemDepo.Sil(işlem);
        }

        public virtual İşlemTipi İşlemTipiAlId(int işlemTipiId)
        {
            if (işlemTipiId == 0)
                return null;
            return _işlemTipiDepo.AlId(işlemTipiId);
        }

        public virtual void İşlemTipiEkle(İşlemTipi işlemTipi)
        {
            if (işlemTipi == null)
                throw new ArgumentNullException("işlemTipi");
            _işlemTipiDepo.Ekle(işlemTipi);
            _önbellekYönetici.KalıpİleSil(ISLEMTIPI_PATTERN_KEY);
        }

        public virtual void İşlemTipiGüncelle(İşlemTipi işlemTipi)
        {
            if (işlemTipi == null)
                throw new ArgumentNullException("işlemTipi");
            _işlemTipiDepo.Güncelle(işlemTipi);
            _önbellekYönetici.KalıpİleSil(ISLEMTIPI_PATTERN_KEY);
        }

        public virtual void İşlemTipiSil(İşlemTipi işlemTipi)
        {
            if (işlemTipi == null)
                throw new ArgumentNullException("işlemTipi");
            _işlemTipiDepo.Sil(işlemTipi);
            _önbellekYönetici.KalıpİleSil(ISLEMTIPI_PATTERN_KEY);
        }
    }
}
