using Core.Domain.Kullanıcılar;

namespace Core.Domain.Güvenlik
{
    public partial class AclKaydı : TemelVarlık
    {
        public int VarlıkId { get; set; }

        public string VarlıkAdı { get; set; }

        public int KullanıcıRolId { get; set; }

        public virtual KullanıcıRolü KullanıcıRolü { get; set; }
    }
}
