using Core.Domain.Kullanıcılar;
using System;

namespace Core.Domain.Forum
{
    public partial class ForumGirdisi : TemelVarlık
    {
        public int SayfaId { get; set; }
        public int KullanıcıId { get; set; }
        public string Yazı { get; set; }
        public string IPAdresi { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public DateTime GüncellenmeTarihi { get; set; }
        public int OySayısı { get; set; }
        public virtual ForumSayfası ForumSayfası { get; set; }
        public virtual Kullanıcı Kullanıcı { get; set; }

    }
}
