using System;
using System.Collections.Generic;

namespace Core.Domain.Forum
{
    public partial class ForumGrubu : TemelVarlık
    {
        private ICollection<Forum> _forum;
        public string Adı { get; set; }
        public int GörüntülenmeSırası { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public DateTime GüncellenmeTarihi { get; set; }
        public virtual ICollection<Forum> Forumlar
        {
            get { return _forum ?? (_forum = new List<Forum>()); }
            protected set { _forum = value; }
        }
    }
}