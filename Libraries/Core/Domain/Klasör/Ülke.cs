using Core.Domain.Siteler;
using System.Collections.Generic;

namespace Core.Domain.Klasör
{
    public partial class Ülke:TemelVarlık,ISiteMappingDestekli
    {
        public string Adı { get; set; }
        public string İkiHarfIsoKodu { get; set; }
        public string ÜçHarfIsoKodu { get; set; }
        public int NumerikIsoKodu { get; set; }
        public bool Yayınlandı { get; set; }
        public int GörüntülenmeSırası { get; set; }
        public bool SitelerdeSınırlı { get; set; }
    }
}

