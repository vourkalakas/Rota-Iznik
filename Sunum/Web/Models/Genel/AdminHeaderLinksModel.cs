using Web.Framework.Mvc.Models;

namespace Web.Models.Genel
{
    public partial class AdminHeaderLinksModel : TemelModel
    {
        public string KimliğeBürünmüşKulllanıcıAdı { get; set; }
        public bool KullanıcıKimliğeBüründü { get; set; }
        public bool AdminLinkGörüntüle { get; set; }
        public string SayfayıDüzenle { get; set; }
    }
}
