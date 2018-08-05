using Core.Domain.Kullanıcılar;
using System;

namespace Core.Domain.Forum
{
    public partial class ForumAboneliği : TemelVarlık
    {
        public Guid AbonelikGuid { get; set; }
        public int KullanıcıId { get; set; }
        public int ForumId { get; set; }
        public int SayfaId { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public virtual Kullanıcı Kullanıcı { get; set; }
    }
}
