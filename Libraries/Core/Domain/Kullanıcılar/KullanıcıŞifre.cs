using System;

namespace Core.Domain.Kullanıcılar
{
    public partial class KullanıcıŞifre : TemelVarlık
    {
        public KullanıcıŞifre()
        {
            this.ŞifreFormatı = ŞifreFormatı.Temiz;
        }
        public int KullanıcıId { get; set; }
        public string Şifre { get; set; }
        public int ŞifreFormatId { get; set; }
        public string ŞifreSalt { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public ŞifreFormatı ŞifreFormatı
        {
            get { return (ŞifreFormatı)ŞifreFormatId; }
            set { this.ŞifreFormatId = (int)value; }
        }
        public virtual Kullanıcı Kullanıcı { get; set; }
    }
}
