using Core.Domain.Güvenlik;
using Core.Domain.Seo;
using Core.Domain.Siteler;

namespace Core.Domain.Sayfalar
{
    public partial class Sayfa : TemelVarlık, ISlugDestekli, ISiteMappingDestekli, IAclDestekli
    {
        public string SistemAdı { get; set; }
        public bool SiteHaritasınaDahil { get; set; }
        public bool ÜstMenüyeDahil { get; set; }
        public bool AltbilgiSütununaDahil1 { get; set; }
        public bool AltbilgiSütununaDahil2 { get; set; }
        public bool AltbilgiSütununaDahil3 { get; set; }
        public int GörüntülenmeSırası { get; set; }
        public bool SiteKapalıykenErişilebilir { get; set; }
        public bool ŞifreKorumalı { get; set; }
        public string Şifre { get; set; }
        public string Başlık { get; set; }
        public string Gövde { get; set; }
        public bool Yayınlandı { get; set; }
        public int SayfaTemaId { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public bool AclKonusu { get; set; }
        public bool SitelerdeSınırlı { get; set; }
    }
}
