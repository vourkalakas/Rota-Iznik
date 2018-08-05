using Web.Framework.Mvc.Models;

namespace Web.Models.Genel
{
    public partial class HeaderLinksModel : TemelModel
    {
        public bool Yetkilendirildi { get; set; }
        public string KullanıcıAdı { get; set; }

        public bool SepetEtkin { get; set; }
        public int SepetOgeleri { get; set; }

        public bool ÖzelMesajlarİzinli { get; set; }
        public string OkunmamışÖzelMesajlar { get; set; }
        public string MesajUyarısı { get; set; }
    }
}
