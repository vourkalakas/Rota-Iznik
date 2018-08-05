using System;

namespace Core.Domain.Forum
{
    public partial class Forum : TemelVarlık
    {
        public int ForumGrubuId { get; set; }
        public string Adı { get; set; }
        public string Açıklama { get; set; }
        public int SayfaSayısı { get; set; }
        public int PostSayısı { get; set; }
        public int SonSayfaId { get; set; }
        public int SonPostId { get; set; }
        public int SonPustKullanıcıId { get; set; }
        public DateTime? SonPostZamanı { get; set; }
        public int GörüntülenmeSırası { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public DateTime GüncellenmeTarihi { get; set; }
        public virtual ForumGrubu ForumGrubu { get; set; }
    }
}
