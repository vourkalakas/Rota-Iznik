using System.Collections.Generic;
using Core.Domain.Kullanıcılar;

namespace Core.Domain.Güvenlik
{
    public partial class İzinKaydı : TemelVarlık
    {
        private ICollection<KullanıcıRolü> _kullanıcıRolleri;
        public string Adı { get; set; }
        public string SistemAdı { get; set; }
        public string Kategori { get; set; }
        public virtual ICollection<KullanıcıRolü> KullanıcıRolleri
        {
            get { return _kullanıcıRolleri ?? (_kullanıcıRolleri = new List<KullanıcıRolü>()); }
            protected set { _kullanıcıRolleri = value; }
        }
    }
}
