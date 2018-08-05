using Core.Domain.Seo;
using System;

namespace Core.Domain.Katalog
{
    public partial class Kategori:TemelVarlık,ISlugDestekli
    {
        public string Adı { get; set; }
        public string Açıklama { get; set; }
        public int KategoriTemaId { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public int ParentKategoriId { get; set; }
        public int ResimId { get; set; }
        public int SayfaBüyüklüğü { get; set; }
        public bool KullanıcılarSayfaBüyüklüğüSeçebilir { get; set; }
        public string SayfaBüyüklüğüSeçenekleri { get; set; }
        public bool AnasayfadaGöster { get; set; }
        public bool ÜstMenüyeEkle { get; set; }
        public bool Yayınlandı { get; set; }
        public bool Silindi { get; set; }
        public int GörüntülenmeSırası { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public DateTime GüncellenmeTarihi { get; set; }

    }
}
