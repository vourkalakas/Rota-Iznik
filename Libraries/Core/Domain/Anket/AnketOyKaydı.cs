using Core.Domain.Kullanıcılar;
using System;

namespace Core.Domain.Anket
{
    public partial class AnketOyKaydı : TemelVarlık
    {
        public int AnketCevapId { get; set; }

        public int KullanıcıId { get; set; }

        public DateTime OluşturulmaTarihi { get; set; }

        public virtual Kullanıcı Kullanıcı { get; set; }

        public virtual AnketCevabı AnketCevabı { get; set; }
    }
}
