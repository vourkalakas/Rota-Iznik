using System;
namespace Core.Domain.Forum
{
    public partial class ForumGirdisiOyu : TemelVarlık
    {
        public int ForumGirdisiId { get; set; }
        public int KullanıcıId { get; set; }
        public bool Yukarı { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public virtual ForumGirdisi ForumGirdisi { get; set; }
    }
}
