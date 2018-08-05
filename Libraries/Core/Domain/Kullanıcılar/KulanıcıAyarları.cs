using Core.Yapılandırma;
using Core.Domain.Kullanıcılar;

namespace Core.Domain.Kullanıcılar
{
    public class KullanıcıAyarları : IAyarlar
    {
        public bool KullanıcıAdlarıEtkin { get; set; }
        public bool KullanıcıAdıUygunluğunuKontrolEt { get; set; }
        public bool KullanıcıAdlarıDeğiştirilebilsin { get; set; }
        public ŞifreFormatı VarsayılanŞifreFormatı { get; set; }
        public string HashŞifreFormatı { get; set; }
        public int MinŞifreUzunluğu { get; set; }
        public int YinelenmemişŞifreFormatı { get; set; }
        public int GeçerliŞifreKurtamaGünü { get; set; }
        public int ŞifreÖmrü { get; set; }
        public int HatalıŞifreDenemesi { get; set; }
        public int HatalıŞifredeKilitDakikası { get; set; }
        public KullanıcıKayıtTipi KullanıcıKayıtTipi { get; set; }
        public bool ProfilResmiYüklenebilir { get; set; }
        public int MaksProfilResmiByte { get; set; }
        public bool VarsayılanProfilResmiEtkin { get; set; }
        public bool KullanıcıKonumlarınıGöster { get; set; }
        public bool KullanıcılarınBağlanmaTarihiniGöster { get; set; }
        public bool ProfillerinGörüntülenebilmesiİzinli { get; set; }
        public bool YeniKullanıcıBildirimi { get; set; }
        public KullanıcıAdıFormatı KullanıcıAdıFormatı { get; set; }
        public bool BültenEtkin { get; set; }
        public bool BültenVarsayılanOlarakTikli { get; set; }
        public bool BültenBloklamayıGizle { get; set; }
        public bool BültenAboneliğiBlocklamayaİzinVer { get; set; }
        public int OnlineKullanıcıDakikaları { get; set; }
        public bool SiteSonZiyaretSayfası { get; set; }
        public bool SilinenKullanıcılarSonek { get; set; }
        public bool EmailİkiDefaGir { get; set; }
        public int KonukKullanıcılarıKaldırmaDakikası { get; set; }

        #region Form alanları

        public bool CinsiyetEtkin { get; set; }
        public bool DoğumTarihiEtkin { get; set; }
        public bool DoğumTarihiGerekli { get; set; }
        public int? DoğumTarihiMinimumYaş { get; set; }
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
        public bool ÜlkeGerekli { get; set; }
        public bool TelEtkin { get; set; }
        public bool TelGerekli { get; set; }
        public bool FaksEtkin { get; set; }
        public bool FaksGerekli { get; set; }
        public bool GizlilikPolitikasıEtkin { get; set; }

        #endregion
    }
}
