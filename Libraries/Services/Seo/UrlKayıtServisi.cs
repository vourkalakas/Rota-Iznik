using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Seo;
using Core.Data;
using Core.Önbellek;

namespace Services.Seo
{
    public partial class UrlKayıtServisi:IUrlKayıtServisi
    {
        private const string URLKAYDI_AKTIF_BY_ID_ADI_KEY = "urlkaydı.aktif.id-ad-{0}-{1}-{2}";
        private const string URLKAYDI_ALL_KEY = "urlkaydı.all";
        private const string URLKAYDI_BY_SLUG_KEY = "urlkaydı.active.slug-{0}";
        private const string URLKAYDI_PATTERN_KEY = "urlkaydı.";
        private readonly IDepo<UrlKaydı> _urlDepo;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public UrlKayıtServisi(IDepo<UrlKaydı> urlDepo,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._urlDepo = urlDepo;
            this._önbellekYönetici = önbellekYönetici;
        }

        [Serializable]
        public class ÖnbellekİçinUrlKaydı
        {
            public int Id { get; set; }
            public int VarlıkId { get; set; }
            public string VarlıkAdı { get; set; }
            public string Slug { get; set; }
            public bool Aktif { get; set; }
        }
        protected ÖnbellekİçinUrlKaydı Map(UrlKaydı kayıt)
        {
            if (kayıt == null)
                throw new ArgumentNullException("kayıt");

            var önbellekİçinUrlKaydı = new ÖnbellekİçinUrlKaydı
            {
                Id = kayıt.Id,
                VarlıkId = kayıt.VarlıkId,
                VarlıkAdı = kayıt.VarlıkAdı,
                Slug = kayıt.Slug,
                Aktif = kayıt.Aktif
            };
            return önbellekİçinUrlKaydı;
        }
        public virtual string AktifSlugAl(int varlıkId, string varlıkAdı)
        {
            string key = string.Format(URLKAYDI_AKTIF_BY_ID_ADI_KEY, varlıkId, varlıkAdı);
            return _önbellekYönetici.Al(key, () =>
            {
                var source = _urlDepo.Tablo;
                var sorgu = from ur in source
                            where ur.VarlıkId == varlıkId &&
                            ur.VarlıkAdı == varlıkAdı &&
                            ur.Aktif
                            orderby ur.Id descending
                            select ur.Slug;
                var slug = sorgu.FirstOrDefault();
                    if (slug == null)
                    slug = "";
                return slug;
            });
        }

        public virtual void SlugKaydet<T>(T varlık, string slug) where T : TemelVarlık, ISlugDestekli
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            int varlıkId = varlık.Id;
            string varlıkAdı = typeof(T).Name;

            var sorgu = from ur in _urlDepo.Tablo
                        where ur.VarlıkId == varlıkId &&
                        ur.VarlıkAdı == varlıkAdı
                        orderby ur.Id descending
                        select ur;
            var tümUrlKayıtları = sorgu.ToList();
            var aktifUrlKaydı = tümUrlKayıtları.FirstOrDefault(x => x.Aktif);

            if (aktifUrlKaydı == null && !string.IsNullOrWhiteSpace(slug))
            {
                var aktifOlmayanÖzelleştirilmişSlug = tümUrlKayıtları
                    .FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.Aktif);
                if (aktifOlmayanÖzelleştirilmişSlug != null)
                {
                    aktifOlmayanÖzelleştirilmişSlug.Aktif = true;
                    UrlKaydıGüncelle(aktifOlmayanÖzelleştirilmişSlug);
                }
                else
                {
                    var urlKaydı = new UrlKaydı
                    {
                        VarlıkId = varlıkId,
                        VarlıkAdı = varlıkAdı,
                        Slug = slug,
                        Aktif = true,
                    };
                    UrlKaydıEkle(urlKaydı);
                }
            }

            if (aktifUrlKaydı != null && string.IsNullOrWhiteSpace(slug))
            {
                aktifUrlKaydı.Aktif = false;
                UrlKaydıGüncelle(aktifUrlKaydı);
            }

            if (aktifUrlKaydı != null && !string.IsNullOrWhiteSpace(slug))
            {
                if (!aktifUrlKaydı.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                {
                    var aktifOlmayanÖzelleştirilmişSlug = tümUrlKayıtları
                        .FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.Aktif);
                    if (aktifOlmayanÖzelleştirilmişSlug != null)
                    {
                        aktifOlmayanÖzelleştirilmişSlug.Aktif = true;
                        UrlKaydıGüncelle(aktifOlmayanÖzelleştirilmişSlug);
                        aktifUrlKaydı.Aktif = false;
                        UrlKaydıGüncelle(aktifUrlKaydı);
                    }
                    else
                    {
                        var urlKaydı = new UrlKaydı
                        {
                            VarlıkId = varlıkId,
                            VarlıkAdı = varlıkAdı,
                            Slug = slug,
                            Aktif = true,
                        };
                        UrlKaydıEkle(urlKaydı);
                        aktifUrlKaydı.Aktif = false;
                        UrlKaydıGüncelle(aktifUrlKaydı);
                    }
                }
            }
        }

        public virtual UrlKaydı SlugİleAl(string slug)
        {
            if (String.IsNullOrEmpty(slug))
                return null;

            var sorgu = from ur in _urlDepo.Tablo
                        where ur.Slug == slug
                        orderby ur.Aktif descending, ur.Id
                        select ur;
            var urlKaydı = sorgu.FirstOrDefault();
            return urlKaydı;
        }

        public virtual ÖnbellekİçinUrlKaydı SlugİleAlÖnbelleklenmiş(string slug)
        {
            if (String.IsNullOrEmpty(slug))
                return null;
            string key = string.Format(URLKAYDI_BY_SLUG_KEY, slug);
            return _önbellekYönetici.Al(key, () =>
            {
                var urlKaydı = SlugİleAl(slug);
                if (urlKaydı == null)
                    return null;

                var önbellekİçinUrlKaydı = Map(urlKaydı);
                return önbellekİçinUrlKaydı;
            });
        }

        public virtual ISayfalıListe<UrlKaydı> TümUrlKayıtlarınıAl(string slug = "", int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _urlDepo.Tablo;
            if (!String.IsNullOrWhiteSpace(slug))
                sorgu = sorgu.Where(ur => ur.Slug.Contains(slug));
            sorgu = sorgu.OrderBy(ur => ur.Slug);

            var urlKayıtları = new SayfalıListe<UrlKaydı>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return urlKayıtları;
        }

        public virtual UrlKaydı UrlKaydıAlId(int urlKayıtId)
        {
            if (urlKayıtId == 0)
                return null;
            return _urlDepo.AlId(urlKayıtId);
        }

        public virtual void UrlKaydıEkle(UrlKaydı urlKaydı)
        {
            if (urlKaydı == null)
                throw new ArgumentNullException("urlKaydı");
            _urlDepo.Ekle(urlKaydı);
            _önbellekYönetici.KalıpİleSil(URLKAYDI_PATTERN_KEY);
        }

        public virtual void UrlKaydıGüncelle(UrlKaydı urlKaydı)
        {
            if (urlKaydı == null)
                throw new ArgumentNullException("urlKaydı");
            _urlDepo.Güncelle(urlKaydı);
            _önbellekYönetici.KalıpİleSil(URLKAYDI_PATTERN_KEY);
        }

        public virtual void UrlKaydınıSil(UrlKaydı urlKaydı)
        {
            if (urlKaydı == null)
                throw new ArgumentNullException("urlKaydı");
            _urlDepo.Sil(urlKaydı);
            _önbellekYönetici.KalıpİleSil(URLKAYDI_PATTERN_KEY);
        }

        public virtual IList<UrlKaydı> UrlKayıtlarınıAlId(int[] urlKayıtIdleri)
        {
            var sorgu = _urlDepo.Tablo;
            return sorgu.Where(p => urlKayıtIdleri.Contains(p.Id)).ToList();
        }

        public virtual void UrlKayıtlarınıSil(IList<UrlKaydı> urlKayıtları)
        {
            if (urlKayıtları == null)
                throw new ArgumentNullException("urlKayıtları");
            _urlDepo.Sil(urlKayıtları);
            _önbellekYönetici.KalıpİleSil(URLKAYDI_PATTERN_KEY);
        }
    }
}
