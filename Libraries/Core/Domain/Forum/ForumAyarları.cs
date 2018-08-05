using Core.Yapılandırma;

namespace Core.Domain.Forum
{
    public class ForumAyarları : IAyarlar
    {
        public bool ForumEtkin { get; set; }
        public bool BağılTarihFormatı { get; set; }
        public bool KullanıcılarınGirdiDüzenlemesiEtkin { get; set; }
        public bool KullanıcılarınAbonelikleriYönetmesiEtkin { get; set; }
        public bool ZiyaretçilerinGirdiEklemesiEtkin { get; set; }
        public bool ZiyaretçilerinSayfaEklemesiEtkin { get; set; }
        public bool KullanıcılarınGirdiSilmesiEtkin { get; set; }
        public bool GirdiOylamaEtkin { get; set; }
        public int GünlükOySınırı { get; set; }
        public int SayfaKonusuMaksimumUzunluğu { get; set; }
        public int StrippedSayfaMaksimumUzunluğu { get; set; }
        public int GirdiMaksimumUzunluğu { get; set; }
        public int SayfaSayfaBüyüklüğü { get; set; }
        public int GirdiSayfaBüyüklüğü { get; set; }
        public int AramaSonuçlarıSayfaBüyüklüğü { get; set; }
        public int AktifTartışmaSayfaBüyüklüğü { get; set; }
        public int SonKullanıcıGirdileriSayfaBüyüklüğü { get; set; }
        public bool KullanıcılarınGirdiSayılarınıGçster { get; set; }
        public EditorTipi ForumEditorü { get; set; }
        public bool İmzaEtkin { get; set; }
        public bool ÖzelMesajEtkin { get; set; }
        public bool ÖzelMesajUyarısıGöster { get; set; }
        public int ÖzelMesajSayfaBüyüklüğü { get; set; }
        public int ForumAboneliğiSayfaBüyüklüğü { get; set; }
        public bool ÖzelMesajlarıBildir { get; set; }
        public int PMKonuMaksimumUzunluğu { get; set; }
        public int PMMesajMaksimumUzunluğu { get; set; }
        public int AnasayfaAktifTartışmaSayfalarıSayısı { get; set; }
        public int AktifTartışmalarBeslemeSayısı { get; set; }
        public bool AktifTartışmalarBeslemeEtkin { get; set; }
        public bool ForumBeslemeleriEtkin { get; set; }
        public int ForumBeslemeleriSayısı { get; set; }
        public int ForumAramaSorgusuMinimumUzunluğu { get; set; }
    }
}
