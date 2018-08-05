using Core.Yapılandırma;

namespace Core.Domain.Blogs
{
    public class BlogAyarları:IAyarlar
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        public bool Etkin { get; set; }

        public int GirdilerSayfaBüyüklüğü { get; set; }

        public bool ZiyaretçilerYorumYapabilir { get; set; }

        public bool YeniBlogYorumunuUyar { get; set; }

        public int TagSayısı { get; set; }

        public bool RssURLBaşlığıGöster { get; set; }

        public bool BlogYorumlarıOnaylanmalı { get; set; }

        public bool BlogYorumlarıTümSitelerdeGöster { get; set; }
    }
}
