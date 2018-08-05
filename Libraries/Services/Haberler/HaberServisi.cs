using Core;
using Core.Data;
using Core.Domain.Haber;
using Core.Domain.Katalog;
using Core.Domain.Siteler;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Haberler
{
    public partial class HaberServisi : IHaberServisi
    {
        #region Fields

        private readonly IDepo<HaberÖğesi> _haberÖğesiDepo;
        private readonly IDepo<HaberYorumu> _haberYorumuDepo;
        private readonly IDepo<SiteMapping> _siteMappingDepo;
        private readonly KatalogAyarları _katalogAyarları;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;

        #endregion

        #region Ctor

        public HaberServisi(IDepo<HaberÖğesi> haberÖğesiDepo,
            IDepo<HaberYorumu> haberYorumuDepo,
            IDepo<SiteMapping> siteMappingDepo,
            KatalogAyarları katalogAyarları,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._haberÖğesiDepo = haberÖğesiDepo;
            this._haberYorumuDepo = haberYorumuDepo;
            this._siteMappingDepo = siteMappingDepo;
            this._katalogAyarları = katalogAyarları;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }

        #endregion

        #region Methods

        #region Haberler

        public virtual void HaberSil(HaberÖğesi haberÖğesi)
        {
            if (haberÖğesi == null)
                throw new ArgumentNullException("haberÖğesi");

            _haberÖğesiDepo.Sil(haberÖğesi);
            _olayYayınlayıcı.OlaySilindi(haberÖğesi);
        }

        public virtual HaberÖğesi HaberAlId(int haberId)
        {
            if (haberId == 0)
                return null;

            return _haberÖğesiDepo.AlId(haberId);
        }

        public virtual IList<HaberÖğesi> HaberAlIdler(int[] haberIdleri)
        {
            var sorgu = _haberÖğesiDepo.Tablo;
            return sorgu.Where(p => haberIdleri.Contains(p.Id)).ToList();
        }

        public virtual ISayfalıListe<HaberÖğesi> TümHaberleriAl(int siteId = 0,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false)
        {
            var sorgu = _haberÖğesiDepo.Tablo;
            if (!gizliOlanıGöster)
            {
                var utcNow = DateTime.UtcNow;
                sorgu = sorgu.Where(n => n.Yayınlandı);
                sorgu = sorgu.Where(n => !n.BaşlangıçTarihi.HasValue || n.BaşlangıçTarihi <= utcNow);
                sorgu = sorgu.Where(n => !n.BitişTarihi.HasValue || n.BitişTarihi >= utcNow);
            }
            sorgu = sorgu.OrderByDescending(n => n.BaşlangıçTarihi ?? n.OluşturulmaTarihi);
            if (siteId > 0 && !_katalogAyarları.IgnoreStoreLimitations)
            {
                sorgu = from n in sorgu
                        join sm in _siteMappingDepo.Tablo
                        on new { c1 = n.Id, c2 = "HaberÖğesi" } equals new { c1 = sm.VarlıkId, c2 = sm.VarlıkAdı } into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.SitelerdeSınırlı || siteId == sm.SiteId
                        select n;
                sorgu = from n in sorgu
                        group n by n.Id
                        into nGroup
                        orderby nGroup.Key
                        select nGroup.FirstOrDefault();
                sorgu = sorgu.OrderByDescending(n => n.BaşlangıçTarihi ?? n.OluşturulmaTarihi);
            }

            var haberler = new SayfalıListe<HaberÖğesi>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return haberler;
        }

        public virtual void HaberEkle(HaberÖğesi haberler)
        {
            if (haberler == null)
                throw new ArgumentNullException("haberler");

            _haberÖğesiDepo.Ekle(haberler);
            _olayYayınlayıcı.OlayEklendi(haberler);
        }

        public virtual void HaberGüncelle(HaberÖğesi haberler)
        {
            if (haberler == null)
                throw new ArgumentNullException("haberler");

            _haberÖğesiDepo.Güncelle(haberler);
            _olayYayınlayıcı.OlayGüncellendi(haberler);
        }

        #endregion

        #region Haber yorumları

        public virtual IList<HaberYorumu> TümYorumlarıAl(int kullanıcıId = 0, int siteId = 0, int? HaberÖğesiId = null,
            bool? onaylandı = null, DateTime? tarihinden = null, DateTime? tarihine = null, string yorumYazısı = null)
        {
            var sorgu = _haberYorumuDepo.Tablo;

            if (onaylandı.HasValue)
                sorgu = sorgu.Where(comment => comment.Onaylandı == onaylandı);

            if (HaberÖğesiId > 0)
                sorgu = sorgu.Where(comment => comment.HaberÖğesiId == HaberÖğesiId);

            if (kullanıcıId > 0)
                sorgu = sorgu.Where(comment => comment.KullanıcıId == kullanıcıId);

            if (siteId > 0)
                sorgu = sorgu.Where(comment => comment.SiteId == siteId);

            if (tarihinden.HasValue)
                sorgu = sorgu.Where(comment => tarihinden.Value <= comment.OluşturulmaTarihi);

            if (tarihine.HasValue)
                sorgu = sorgu.Where(comment => tarihine.Value >= comment.OluşturulmaTarihi);

            if (!string.IsNullOrEmpty(yorumYazısı))
                sorgu = sorgu.Where(c => c.YorumYazısı.Contains(yorumYazısı) || c.YorumBaşlığı.Contains(yorumYazısı));

            sorgu = sorgu.OrderBy(nc => nc.OluşturulmaTarihi);

            return sorgu.ToList();
        }

        public virtual HaberYorumu YorumAlId(int haberYorumuId)
        {
            if (haberYorumuId == 0)
                return null;

            return _haberYorumuDepo.AlId(haberYorumuId);
        }

        public virtual IList<HaberYorumu> YorumAlIdler(int[] yorumIdleri)
        {
            if (yorumIdleri == null || yorumIdleri.Length == 0)
                return new List<HaberYorumu>();

            var sorgu = from nc in _haberYorumuDepo.Tablo
                        where yorumIdleri.Contains(nc.Id)
                        select nc;
            var yorumlar = sorgu.ToList();
            var sıralıYorumlar = new List<HaberYorumu>();
            foreach (int id in yorumIdleri)
            {
                var yorum = yorumlar.Find(x => x.Id == id);
                if (yorum != null)
                    sıralıYorumlar.Add(yorum);
            }
            return sıralıYorumlar;
        }

        public virtual int YorumSayısı(HaberÖğesi HaberÖğesi, int sited = 0, bool? onaylandı = null)
        {
            var sorgu = _haberYorumuDepo.Tablo.Where(comment => comment.HaberÖğesiId == HaberÖğesi.Id);

            if (sited > 0)
                sorgu = sorgu.Where(comment => comment.SiteId == sited);

            if (onaylandı.HasValue)
                sorgu = sorgu.Where(comment => comment.Onaylandı == onaylandı.Value);

            return sorgu.Count();
        }

        public virtual void yorumSil(HaberYorumu haberYorumu)
        {
            if (haberYorumu == null)
                throw new ArgumentNullException("haberYorumu");

            _haberYorumuDepo.Sil(haberYorumu);
            _olayYayınlayıcı.OlaySilindi(haberYorumu);
        }

        public virtual void yorumlarıSil(IList<HaberYorumu> haberYorumları)
        {
            if (haberYorumları == null)
                throw new ArgumentNullException("haberYorumları");

            foreach (var haberYorumu in haberYorumları)
            {
                yorumSil(haberYorumu);
            }
        }

        #endregion

        #endregion
    }
}
