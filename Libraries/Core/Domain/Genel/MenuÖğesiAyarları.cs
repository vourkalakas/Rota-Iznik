using Core.Yapılandırma;

namespace Core.Domain.Genel
{
    public class MenuÖğesiAyarları : IAyarlar
    {
        public bool AnasayfaMenuÖğesi { get; set; }

        public bool KullanıcıBilgisiMenuÖğesi { get; set; }

        public bool BlogMenuÖğesi { get; set; }

        public bool ForumMenuÖğesi { get; set; }

        public bool İletişimMenuÖğesi { get; set; }
    }
}
