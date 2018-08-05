using System.Collections.Generic;

namespace Web.Framework.Olaylar
{
    public class ÜrünAramaOlayı
    {
        public string AramaTerimi { get; set; }
        public bool AçıklamalardaAra { get; set; }
        public IList<int> KategoriIdleri { get; set; }
        public int DilId { get; set; }
    }
}
