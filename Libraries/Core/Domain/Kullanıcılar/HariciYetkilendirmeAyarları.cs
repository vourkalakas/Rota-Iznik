using Core.Yapılandırma;
using System.Collections.Generic;

namespace Core.Domain.Kullanıcılar
{
    public partial class HariciYetkilendirmeAyarları:IAyarlar
    {
        public HariciYetkilendirmeAyarları()
        {
            AktifYetkilendirmeMetoduSistemAdları = new List<string>();
        }
        public bool OtoKayıtEtkin { get; set; }
        public bool EmailDoğrulamasıGerekli { get; set; }
        public List<string> AktifYetkilendirmeMetoduSistemAdları { get; set; }
    }
}

