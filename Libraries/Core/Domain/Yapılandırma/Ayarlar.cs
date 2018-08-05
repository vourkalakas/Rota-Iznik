using Core.Domain.Localization;

namespace Core.Domain.Yapılandırma
{
    public partial class Ayarlar : TemelVarlık, ILocalizedEntity
    {
        public Ayarlar() { }
        public Ayarlar(string ad, string değer, int siteId = 0)
        {
            this.Ad = ad;
            this.Değer = değer;
            this.SiteId = siteId;
        }
        public string Ad { get; set; }
        public string Değer { get; set; }
        public int SiteId { get; set; }
        public override string ToString()
        {
            return Ad;
        }
    }
}
