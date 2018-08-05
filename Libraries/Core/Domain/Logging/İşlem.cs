using Core.Domain.Kullanıcılar;
using System;

namespace Core.Domain.Logging
{
    public partial class İşlem : TemelVarlık
    {
        public int İşlemTipiId { get; set; }
        public int KullanıcıId { get; set; }
        public string Yorum { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public virtual İşlemTipi İşlemTipi { get; set; }
        public virtual Kullanıcı Kullanıcı { get; set; }
        public virtual string IpAdresi { get; set; }
    }
}
