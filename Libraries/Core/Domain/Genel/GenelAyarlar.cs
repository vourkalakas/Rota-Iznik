using System.Collections.Generic;
using Core.Yapılandırma;

namespace Core.Domain.Genel
{
    public class GenelAyarlar : IAyarlar
    {
        public GenelAyarlar()
        {
            SiteHaritasıÖzelURL = new List<string>();
            GünlükListesiniYoksay = new List<string>();
        }
        public bool İletişimFormuKonuBaşlığı { get; set; }
        public bool İletişimFormuSistemMaili { get; set; }
        public bool StoredProcedureKullanDestekliyse { get; set; }
        public bool StoredProcedureKullanYüklüKategoriler { get; set; }
        public bool SiteHaritasıEtkin { get; set; }
        public bool SiteHaritasındaKategoriler { get; set; }
        public List<string> SiteHaritasıÖzelURL { get; set; }
        public bool JavaScriptKapalıHatası { get; set; }
        public bool TamMetinAramayıKullan { get; set; }
        public TamMetinAramaModu TamMetinModu { get; set; }
        public bool Günlük404Hataları { get; set; }
        public string BreadcrumbSınırlayıcı { get; set; }
        public bool XuaUyumluRender { get; set; }
        public string XuaUyumluDeğer { get; set; }
        public List<string> GünlükListesiniYoksay { get; set; }
        public bool BbcodeEditorYeniPenceredeAçılsın { get; set; }
    }
}
