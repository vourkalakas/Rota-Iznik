using Core;
using Core.Data;
using Core.Domain.Blogs;
using Core.Domain.Katalog;
using Core.Domain.Siteler;
using Services.Blog;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Blog
{
    public partial class BlogServisi : IBlogServisi
    {
        #region Fields

        private readonly IDepo<BlogPost> _blogPostDepo;
        private readonly IDepo<BlogYorumu> _blogYorumuDepo;
        private readonly IDepo<SiteMapping> _siteMappingDepo;
        private readonly KatalogAyarları _katalogAyarları;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;

        #endregion

        #region Ctor

        public BlogServisi(IDepo<BlogPost> _logPostDepo,
            IDepo<BlogYorumu> blogYorumuDepo,
            IDepo<SiteMapping> siteMappingDepo,
            KatalogAyarları katalogAyarları,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._blogPostDepo = _logPostDepo;
            this._blogYorumuDepo = blogYorumuDepo;
            this._siteMappingDepo = siteMappingDepo;
            this._katalogAyarları = katalogAyarları;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }

        #endregion

        #region Methods

        #region Blog

        public virtual void BlogSil(BlogPost blogÖğesi)
        {
            if (blogÖğesi == null)
                throw new ArgumentNullException("blogÖğesi");

            _blogPostDepo.Sil(blogÖğesi);
            _olayYayınlayıcı.OlaySilindi(blogÖğesi);
        }

        public virtual BlogPost BlogAlId(int blogId)
        {
            if (blogId == 0)
                return null;

            return _blogPostDepo.AlId(blogId);
        }

        public virtual IList<BlogPost> BlogAlIdler(int[] blogIdleri)
        {
            var sorgu = _blogPostDepo.Tablo;
            return sorgu.Where(p => blogIdleri.Contains(p.Id)).ToList();
        }

        public virtual ISayfalıListe<BlogPost> TümBloglerıAl(int siteId = 0, DateTime? tarihinden = null, DateTime? tarihine = null,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false)
        {
            var sorgu = _blogPostDepo.Tablo;
            if (tarihinden.HasValue)
                sorgu = sorgu.Where(b => tarihinden.Value <= (b.BaşlangıçTarihi ?? b.OluşturulmaTarihi));
            if (tarihine.HasValue)
                sorgu = sorgu.Where(b => tarihine.Value >= (b.BaşlangıçTarihi ?? b.OluşturulmaTarihi));
            if (!gizliOlanıGöster)
            {
                var utcNow = DateTime.UtcNow;
                sorgu = sorgu.Where(n => !n.BaşlangıçTarihi.HasValue || n.BaşlangıçTarihi <= utcNow);
                sorgu = sorgu.Where(n => !n.BitişTarihi.HasValue || n.BitişTarihi >= utcNow);
            }
            sorgu = sorgu.OrderByDescending(n => n.BaşlangıçTarihi ?? n.OluşturulmaTarihi);
            if (siteId > 0 && !_katalogAyarları.IgnoreStoreLimitations)
            {
                sorgu = from n in sorgu
                        join sm in _siteMappingDepo.Tablo
                        on new { c1 = n.Id, c2 = "BlogPost" } equals new { c1 = sm.VarlıkId, c2 = sm.VarlıkAdı } into n_sm
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

            var blogler = new SayfalıListe<BlogPost>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return blogler;
        }
        public virtual ISayfalıListe<BlogPost> TümBloglerıAlTag(int siteId = 0, string tag = "",
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false)
        {
            tag = tag.Trim();

            var tümBlogPostları = TümBloglerıAl(siteId: siteId, gizliOlanıGöster: gizliOlanıGöster);
            var taglıBlogPostları = new List<BlogPost>();
            foreach (var blogPost in tümBlogPostları)
            {
                var tags = blogPost.TaglarıBirleştir();
                if (!String.IsNullOrEmpty(tags.FirstOrDefault(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase))))
                    taglıBlogPostları.Add(blogPost);
            }
            var result = new SayfalıListe<BlogPost>(taglıBlogPostları, sayfaIndeksi, sayfaBüyüklüğü);
            return result;
        }
        public virtual IList<BlogPostTag> TümTaglarıAl(int siteId, bool gizliOlanıGöster = false)
        {
            var blogPostTagları = new List<BlogPostTag>();

            var blogPostları = TümBloglerıAl(siteId: siteId, gizliOlanıGöster: gizliOlanıGöster);
            foreach (var blogPost in blogPostları)
            {
                var taglar = blogPost.TaglarıBirleştir();
                foreach (string tag in taglar)
                {
                    var bulunanBlogPostTagları = blogPostTagları.Find(bpt => bpt.Adı.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                    if (bulunanBlogPostTagları == null)
                    {
                        bulunanBlogPostTagları = new BlogPostTag
                        {
                            Adı = tag,
                            BlogPostSayısı = 1
                        };
                        blogPostTagları.Add(bulunanBlogPostTagları);
                    }
                    else
                        bulunanBlogPostTagları.BlogPostSayısı++;
                }
            }

            return blogPostTagları;
        }

        public virtual void BlogEkle(BlogPost blogler)
        {
            if (blogler == null)
                throw new ArgumentNullException("blogler");

            _blogPostDepo.Ekle(blogler);
            _olayYayınlayıcı.OlayEklendi(blogler);
        }

        public virtual void BlogGüncelle(BlogPost blogler)
        {
            if (blogler == null)
                throw new ArgumentNullException("blogler");

            _blogPostDepo.Güncelle(blogler);
            _olayYayınlayıcı.OlayGüncellendi(blogler);
        }

        #endregion

        #region Blog yorumları

        public virtual IList<BlogYorumu> TümYorumlarıAl(int kullanıcıId = 0, int siteId = 0, int? BlogPostId = null,
            bool? onaylandı = null, DateTime? tarihinden = null, DateTime? tarihine = null, string yorumYazısı = null)
        {
            var sorgu = _blogYorumuDepo.Tablo;

            if (onaylandı.HasValue)
                sorgu = sorgu.Where(comment => comment.Onaylandı == onaylandı);

            if (BlogPostId > 0)
                sorgu = sorgu.Where(comment => comment.BlogPostId == BlogPostId);

            if (kullanıcıId > 0)
                sorgu = sorgu.Where(comment => comment.KullanıcıId == kullanıcıId);

            if (siteId > 0)
                sorgu = sorgu.Where(comment => comment.SiteId == siteId);

            if (tarihinden.HasValue)
                sorgu = sorgu.Where(comment => tarihinden.Value <= comment.OluşturulmaTarihi);

            if (tarihine.HasValue)
                sorgu = sorgu.Where(comment => tarihine.Value >= comment.OluşturulmaTarihi);

            if (!string.IsNullOrEmpty(yorumYazısı))
                sorgu = sorgu.Where(c => c.YorumYazısı.Contains(yorumYazısı));

            sorgu = sorgu.OrderBy(nc => nc.OluşturulmaTarihi);

            return sorgu.ToList();
        }

        public virtual BlogYorumu YorumAlId(int blogYorumuId)
        {
            if (blogYorumuId == 0)
                return null;

            return _blogYorumuDepo.AlId(blogYorumuId);
        }

        public virtual IList<BlogYorumu> YorumAlIdler(int[] yorumIdleri)
        {
            if (yorumIdleri == null || yorumIdleri.Length == 0)
                return new List<BlogYorumu>();

            var sorgu = from nc in _blogYorumuDepo.Tablo
                        where yorumIdleri.Contains(nc.Id)
                        select nc;
            var yorumlar = sorgu.ToList();
            var sıralıYorumlar = new List<BlogYorumu>();
            foreach (int id in yorumIdleri)
            {
                var yorum = yorumlar.Find(x => x.Id == id);
                if (yorum != null)
                    sıralıYorumlar.Add(yorum);
            }
            return sıralıYorumlar;
        }

        public virtual int YorumSayısı(BlogPost BlogPost, int sited = 0, bool? onaylandı = null)
        {
            var sorgu = _blogYorumuDepo.Tablo.Where(comment => comment.BlogPostId == BlogPost.Id);

            if (sited > 0)
                sorgu = sorgu.Where(comment => comment.SiteId == sited);

            if (onaylandı.HasValue)
                sorgu = sorgu.Where(comment => comment.Onaylandı == onaylandı.Value);

            return sorgu.Count();
        }

        public virtual void yorumSil(BlogYorumu blogYorumu)
        {
            if (blogYorumu == null)
                throw new ArgumentNullException("blogYorumu");

            _blogYorumuDepo.Sil(blogYorumu);
            _olayYayınlayıcı.OlaySilindi(blogYorumu);
        }

        public virtual void yorumlarıSil(IList<BlogYorumu> blogYorumları)
        {
            if (blogYorumları == null)
                throw new ArgumentNullException("blogYorumları");

            foreach (var blogYorumu in blogYorumları)
            {
                yorumSil(blogYorumu);
            }
        }

        #endregion

        #endregion
    }
}
