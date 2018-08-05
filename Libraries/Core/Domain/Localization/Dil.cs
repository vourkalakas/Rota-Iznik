using Core.Domain.Siteler;

namespace Core.Domain.Localization
{
    public partial class Dil : TemelVarlık, ISiteMappingDestekli
    {
        public string Adı { get; set; }
        public string DilKültürü { get; set; }
        public string ÖzelSeoKodu { get; set; }
        public string BayrakResmiDosyaAdı { get; set; }
        public bool Rtl { get; set; }
        public bool SitelerdeSınırlı { get; set; }
        public int VarsayılanDövizId { get; set; }
        public bool Yayınlandı { get; set; }
        public int GörüntülenmeSırası { get; set; }
    }
}
