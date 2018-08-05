using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Data;
using Core.Domain.Genel;
using Core.Domain.Kullanıcılar;
using Core.Domain.Logging;
using Data;

namespace Services.Logging
{
    public partial class VarsayılanLogger : ILogger
    {
        #region Fields

        private readonly IDepo<Log> _logDepo;
        private readonly IWebYardımcısı _webYardımcısı;
        private readonly IDbContext _dbContext;
        private readonly IDataSağlayıcı _dataSağlayıcı;
        private readonly GenelAyarlar _genelAyarlar;

        #endregion

        #region Ctor
        public VarsayılanLogger(IDepo<Log> logDepo,
            IWebYardımcısı webYardımcısı,
            IDbContext dbContext,
            IDataSağlayıcı dataSağlayıcı,
            GenelAyarlar genelAyarlar)
        {
            this._logDepo = logDepo;
            this._webYardımcısı = webYardımcısı;
            this._dbContext = dbContext;
            this._dataSağlayıcı = dataSağlayıcı;
            this._genelAyarlar = genelAyarlar;
        }

        #endregion

        #region Utitilities
        protected virtual bool GünlüğüYoksay(string mesaj)
        {
            if (!_genelAyarlar.GünlükListesiniYoksay.Any())
                return false;

            if (String.IsNullOrWhiteSpace(mesaj))
                return false;

            return _genelAyarlar
                .GünlükListesiniYoksay
                .Any(x => mesaj.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        #endregion

        #region Methods
        public virtual bool Etkin(LogSeviyesi level)
        {
            switch (level)
            {
                case LogSeviyesi.Debug:
                    return false;
                default:
                    return true;
            }
        }
        public virtual void LogSil(Log log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            _logDepo.Sil(log);
        }
        public virtual void LoglarıSil(IList<Log> loglar)
        {
            if (loglar == null)
                throw new ArgumentNullException("logs");

            _logDepo.Sil(loglar);
        }
        public virtual void LogTemizle()
        {
            if (_genelAyarlar.StoredProcedureKullanDestekliyse && _dataSağlayıcı.StoredProceduredDestekli)
            {
                //tüm veritabanları "TRUNCATE" seçeneğini destekliyor mu?
                string logTableName = _dbContext.GetTableName<Log>();
                _dbContext.SqlKomutunuÇalıştır(String.Format("TRUNCATE TABLE [{0}]", logTableName));
            }
            else
            {
                var log = _logDepo.Tablo.ToList();
                foreach (var logItem in log)
                    _logDepo.Sil(logItem);
            }
        }
        public virtual ISayfalıListe<Log> TümLoglarıAl(DateTime? ŞuTarihten = null, DateTime? ŞuTarihe = null,
            string mesaj = "", LogSeviyesi? logSeviyesi = null,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _logDepo.Tablo;
            if (ŞuTarihten.HasValue)
                sorgu = sorgu.Where(l => ŞuTarihten.Value <= l.OluşturulmaTarihi);
            if (ŞuTarihe.HasValue)
                sorgu = sorgu.Where(l => ŞuTarihe.Value >= l.OluşturulmaTarihi);
            if (logSeviyesi.HasValue)
            {
                var logLevelId = (int)logSeviyesi.Value;
                sorgu = sorgu.Where(l => logLevelId == l.LogSeviyeId);
            }
            if (!String.IsNullOrEmpty(mesaj))
                sorgu = sorgu.Where(l => l.KısaMesaj.Contains(mesaj) || l.TamMesaj.Contains(mesaj));
            sorgu = sorgu.OrderByDescending(l => l.OluşturulmaTarihi);

            var log = new SayfalıListe<Log>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return log;
        }
        public virtual Log LogAlId(int logId)
        {
            if (logId == 0)
                return null;

            return _logDepo.AlId(logId);
        }
        public virtual IList<Log> LoglarıAlId(int[] logIdleri)
        {
            if (logIdleri == null || logIdleri.Length == 0)
                return new List<Log>();

            var sorgu = from l in _logDepo.Tablo
                        where logIdleri.Contains(l.Id)
                        select l;
            var logParçaları = sorgu.ToList();
            //geçilen tanımlayıcılara göre sırala
            var sıralananLogParçaları = new List<Log>();
            foreach (int id in logIdleri)
            {
                var log = logParçaları.Find(x => x.Id == id);
                if (log != null)
                    sıralananLogParçaları.Add(log);
            }
            return sıralananLogParçaları;
        }
        public virtual Log LogEkle(LogSeviyesi logSeviyesi, string kısaMesaj, string tamMesaj = "", Kullanıcı kullanıcı = null)
        {
            //check ignore word/phrase list?
            if (GünlüğüYoksay(kısaMesaj) || GünlüğüYoksay(tamMesaj))
                return null;

            var log = new Log
            {
                LogSeviyesi = logSeviyesi,
                KısaMesaj = kısaMesaj,
                TamMesaj = tamMesaj,
                IpAdresi = _webYardımcısı.MevcutIpAdresiAl(),
                Kullanıcı = kullanıcı,
                SayfaUrl = _webYardımcısı.SayfanınUrlsiniAl(true),
                YönlendirenURL = _webYardımcısı.UrlYönlendiriciAl(),
                OluşturulmaTarihi = DateTime.UtcNow
            };

            _logDepo.Ekle(log);

            return log;
        }

        #endregion
    }
}
