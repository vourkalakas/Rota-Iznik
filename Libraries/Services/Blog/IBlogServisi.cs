using Core;
using Core.Domain.Blogs;
using System;
using System.Collections.Generic;

namespace Services.Blog
{
    public partial interface IBlogServisi
    {
        #region Blog

        void BlogSil(BlogPost blogÖğesi);

        BlogPost BlogAlId(int blogId);

        IList<BlogPost> BlogAlIdler(int[] blogIdleri);

        ISayfalıListe<BlogPost> TümBloglerıAl(int siteId = 0, DateTime? tarihinden = null, DateTime? tarihine = null,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false);
        ISayfalıListe<BlogPost> TümBloglerıAlTag(int siteId = 0, string tag = "",
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false);

        IList<BlogPostTag> TümTaglarıAl(int siteId, bool gizliOlanıGöster = false);

        void BlogEkle(BlogPost blogler);

        void BlogGüncelle(BlogPost blogler);

        #endregion

        #region Blog yorumları

        IList<BlogYorumu> TümYorumlarıAl(int kullanıcıId = 0, int siteId = 0, int? BlogÖğesiId = null,
            bool? onaylandı = null, DateTime? tarihinden = null, DateTime? tarihine = null, string yorumYazısı = null);

        BlogYorumu YorumAlId(int blogYorumuId);

        IList<BlogYorumu> YorumAlIdler(int[] yorumIdleri);

        int YorumSayısı(BlogPost BlogPost, int sited = 0, bool? onaylandı = null);

        void yorumSil(BlogYorumu blogYorumu);

        void yorumlarıSil(IList<BlogYorumu> blogYorumları);

        #endregion
    }
}
