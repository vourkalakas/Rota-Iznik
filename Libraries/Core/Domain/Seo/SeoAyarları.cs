using System.Collections.Generic;
using Core.Yapılandırma;

namespace Core.Domain.Seo
{
    public class SeoAyarları : IAyarlar
    {
        public string SayfaBaşlığıAyırıcısı { get; set; }
        public SayfaBaşlığıSeoAyarı SayfaBaşlığıSeoAyarı { get; set; }
        public string VarsayılanBaşlık { get; set; }
        public string VarsayılanMetaKeywordler { get; set; }
        public string VarsayılanMetaDescription { get; set; }
        public bool UnicodeURLIzinver { get; set; }
        public bool CanonicalUrlIzinVer { get; set; }
        public WwwGerekliliği WwwGerekliliği { get; set; }
        public bool JSPaketlemeyeIzinVer { get; set; }
        public bool CssPaketlemeyeIzinVer { get; set; }
        public bool TwitterMetaTagları { get; set; }
        public bool OpenGraphMetaTagları { get; set; }
        public List<string> AyrılmışURLKayıtÇekirgeği { get; set; }
        public string ÖzelHeadTagları { get; set; }
        public bool BatıOlmayanKarakterleriDönüştür { get; set; }
        public bool URLlerdeUnicodeKarakterlerineİzinVer { get; set; }
        public List<string> ReserveUrlKayıtSlugları { get; set; }
    }
}
