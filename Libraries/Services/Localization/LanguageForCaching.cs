using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Önbellek;
using Core.Domain.Localization;

namespace Services.Localization
{
    [Serializable]
    [NotMapped]
    public class LanguageForCaching : Dil, IÖnbellekİçinVarlık
    {
        public LanguageForCaching()
        {

        }
        public LanguageForCaching(Dil l)
        {
            Id = l.Id;
            Adı = l.Adı;
            DilKültürü = l.DilKültürü;
            ÖzelSeoKodu = l.ÖzelSeoKodu;
            BayrakResmiDosyaAdı = l.BayrakResmiDosyaAdı;
            Rtl = l.Rtl;
            SitelerdeSınırlı = l.SitelerdeSınırlı;
            VarsayılanDövizId = l.VarsayılanDövizId;
            Yayınlandı = l.Yayınlandı;
            GörüntülenmeSırası = l.GörüntülenmeSırası;
        }
    }
}
