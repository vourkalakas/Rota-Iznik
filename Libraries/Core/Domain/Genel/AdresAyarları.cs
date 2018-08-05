using Core.Yapılandırma;

namespace Core.Domain.Genel
{
    public class AdresAyarları : IAyarlar
    {
        public bool ŞirketEtkin { get; set; }
        public bool ŞirketGerekli { get; set; }
        public bool SokakAdresiEtkin { get; set; }
        public bool SokakAdresiGerekli { get; set; }
        public bool SokakAdresi2Etkin { get; set; }
        public bool SokakAdresi2Gerekli { get; set; }
        public bool PostaKoduEtkin { get; set; }
        public bool PostaKoduGerekli { get; set; }
        public bool ŞehirEtkin { get; set; }
        public bool ŞehirGerekli { get; set; }
        public bool ÜlkeEtkin { get; set; }
        public bool TelEtkin { get; set; }
        public bool TelGerekli { get; set; }
        public bool FaksEtkin { get; set; }
        public bool FaksGerekli { get; set; }
    }
}
