using Core.Domain.Kullanıcılar;
using System;

namespace Core.Domain.Forum
{
    public partial class ForumSayfası : TemelVarlık
    {
        public int ForumId { get; set; }
        public int KullanıcıId { get; set; }
        public int SayfaTipiId { get; set; }
        public string Konu { get; set; }
        public int PostSayısı { get; set; }
        public int Görüntülenme { get; set; }
        public int SonPostId { get; set; }
        public int SonPostKullanıcıId { get; set; }
        public DateTime? SonPostZamanı { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public DateTime GüncellenmeTarihi { get; set; }
        public ForumSayfasıTipi ForumSayfasıTipi
        {
            get
            {
                return (ForumSayfasıTipi)this.SayfaTipiId;
            }
            set
            {
                this.SayfaTipiId = (int)value;
            }
        }
        public virtual Forum Forum { get; set; }
        public virtual Kullanıcı Kullanıcı { get; set; }
        public int CevapSayısı
        {
            get
            {
                int result = 0;
                if (PostSayısı > 0)
                    result = PostSayısı - 1;
                return result;
            }
        }
    }
}
