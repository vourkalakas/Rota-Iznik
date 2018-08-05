using Core.Yapılandırma;

namespace Services.Yardımcılar
{
    public class TarihAyarları : IAyarlar
    {
        public string SiteVarsayılanZamanDilimiId { get; set; }
        public bool KullanıcıZamanDilimiAyarlamasıİzinli { get; set; }
    }
}
