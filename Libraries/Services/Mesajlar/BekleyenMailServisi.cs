using System;
using System.Collections.Generic;
using Core;
using Core.Domain.Mesajlar;
using Core.Data;
using Data;
using Core.Domain.Genel;
using Services.Olaylar;
using System.Linq;

namespace Services.Mesajlar
{
    public partial class BekleyenMailServisi : IBekleyenMailServisi
    {
        private readonly IDepo<BekleyenMail> _bekleyenDepo;
        private readonly IDbContext _dbContext;
        private readonly IDataSağlayıcı _dataSağlayıcı;
        private readonly GenelAyarlar _genelAyarlar;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        public BekleyenMailServisi(IDepo<BekleyenMail> bekleyenDepo,
            IDbContext dbContext,
            IDataSağlayıcı dataSağlayıcı,
            GenelAyarlar genelAyarlar,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._bekleyenDepo = bekleyenDepo;
            this._dbContext = dbContext;
            this._dataSağlayıcı = dataSağlayıcı;
            this._genelAyarlar = genelAyarlar;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }
        public virtual BekleyenMail BekleyenMailAlId(int bekleyenMailId)
        {
            if (bekleyenMailId == 0)
                return null;
            return _bekleyenDepo.AlId(bekleyenMailId);
        }

        public virtual void BekleyenMailEkle(BekleyenMail bekleyenMail)
        {
            if (bekleyenMail == null)
                throw new ArgumentNullException("bekleyenMail");
            _bekleyenDepo.Ekle(bekleyenMail);
            _olayYayınlayıcı.OlayEklendi(bekleyenMail);

        }

        public virtual void BekleyenMailGüncelle(BekleyenMail bekleyenMail)
        {
            if (bekleyenMail == null)
                throw new ArgumentNullException("bekleyenMail");
            _bekleyenDepo.Güncelle(bekleyenMail);
            _olayYayınlayıcı.OlayGüncellendi(bekleyenMail);
        }

        public virtual IList<BekleyenMail> BekleyenMailleriAlId(int[] bekleyenMailIds)
        {
            if (bekleyenMailIds == null || bekleyenMailIds.Length == 0)
                return new List<BekleyenMail>();
            var sorgu = from qe in _bekleyenDepo.Tablo
                        where bekleyenMailIds.Contains(qe.Id)
                        select qe;
            var bekleyenMailler = sorgu.ToList();
            var sıralananMailler = new List<BekleyenMail>();
            foreach (int id in bekleyenMailIds)
            {
                var bekleyenMail = bekleyenMailler.Find(x => x.Id == id);
                if (bekleyenMail != null)
                    sıralananMailler.Add(bekleyenMail);
            }
            return sıralananMailler;
        }

        public virtual void BekleyenMailleriSil(IList<BekleyenMail> bekleyenMailler)
        {
            if (bekleyenMailler == null)
                throw new ArgumentNullException("bekleyenMail");
            _bekleyenDepo.Sil(bekleyenMailler);
            foreach (var bekleyenMail in bekleyenMailler)
                _olayYayınlayıcı.OlaySilindi(bekleyenMail);
        }

        public virtual void BekleyenMailSil(BekleyenMail bekleyenMail)
        {
            if (bekleyenMail == null)
                throw new ArgumentNullException("bekleyenMail");
            _bekleyenDepo.Sil(bekleyenMail);
            _olayYayınlayıcı.OlaySilindi(bekleyenMail);
        }

        public virtual ISayfalıListe<BekleyenMail> EmailleriAra(string emailden, string emaile, DateTime? oluşturulmaTarihinden, DateTime? oluşturulmaTarihine, bool gönderilmemişÖğeler, bool gönderilmişÖğeler, int maksDenemeSüresi, bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            emailden = (emailden ?? String.Empty).Trim();
            emaile = (emaile ?? String.Empty).Trim();

            var sorgu = _bekleyenDepo.Tablo;
            if (!String.IsNullOrEmpty(emailden))
                sorgu = sorgu.Where(qe => qe.Kimden.Contains(emailden));
            if (!String.IsNullOrEmpty(emaile))
                sorgu = sorgu.Where(qe => qe.Kime.Contains(emaile));
            if (oluşturulmaTarihinden.HasValue)
                sorgu = sorgu.Where(qe => qe.OluşturulmaTarihi >= oluşturulmaTarihinden);
            if (oluşturulmaTarihine.HasValue)
                sorgu = sorgu.Where(qe => qe.OluşturulmaTarihi <= oluşturulmaTarihine);
            if (gönderilmemişÖğeler)
                sorgu = sorgu.Where(qe => !qe.TarihindeGönderildi.HasValue);
            if (gönderilmişÖğeler)
            {
                var nowUtc = DateTime.UtcNow;
                sorgu = sorgu.Where(qe => !qe.ŞuTarihdenÖnceGönderme.HasValue || qe.ŞuTarihdenÖnceGönderme.Value <= nowUtc);
            }
            sorgu = sorgu.Where(qe => qe.GöndermeDenemesi < maksDenemeSüresi);
            sorgu = enYeniler ?
                sorgu.OrderByDescending(qe => qe.OluşturulmaTarihi) :
                sorgu.OrderByDescending(qe => qe.ÖncelikId).ThenBy(qe => qe.OluşturulmaTarihi);

            var bekleyenMailler = new SayfalıListe<BekleyenMail>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return bekleyenMailler;
        }

        public virtual void TümEmailleriSil()
        {
            if (_genelAyarlar.StoredProcedureKullanDestekliyse && _dataSağlayıcı.StoredProceduredDestekli)
            {
                string bekleyenMaillerTabloAdı = _dbContext.GetTableName<BekleyenMail>();
                _dbContext.SqlKomutunuÇalıştır(String.Format("TRUNCATE TABLE [{0}]", bekleyenMaillerTabloAdı));
            }
            else
            {
                var bekleyenMailler = _bekleyenDepo.Tablo.ToList();
                foreach (var qe in bekleyenMailler)
                    _bekleyenDepo.Sil(qe);
            }
        }
    }
}

