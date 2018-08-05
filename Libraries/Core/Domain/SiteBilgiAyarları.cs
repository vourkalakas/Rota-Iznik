using Core.Yapılandırma;

namespace Core.Domain
{
    public class SiteBilgiAyarları : IAyarlar
    {
        public bool SiteKapalı { get; set; }
        public int LogoResimId { get; set; }
        public string MevcutSiteTeması { get; set; }
        public bool KullanıcılarTemaSeçebilsin { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string YoutubeLink { get; set; }
        public string GooglePlusLink { get; set; }
        //public bool MiniProfilerGörüntüle { get; set; }
    }
}
