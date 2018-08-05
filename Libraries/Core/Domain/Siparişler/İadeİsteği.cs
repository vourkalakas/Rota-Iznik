using System;
using Core.Domain.Kullanıcılar;

namespace Core.Domain.Siparişler
{
    public partial class İadeİsteği : TemelVarlık
    {
        public string ÖzelNumara { get; set; }
        public int SiteId { get; set; }
        public int SiparişÖğesiId { get; set; }
        public int KullanıcıId { get; set; }
        public int Miktar { get; set; }
        public string İadeSebebi { get; set; }
        public string İstenenEylem { get; set; }
        public string KullanıcıYorumları { get; set; }
        public int YüklenenDosyaId { get; set; }
        public string PersonelNotları { get; set; }
        public int İadeİsteğiDurumuId { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public DateTime GüncellenmeTarihi { get; set; }
        public İadeİsteğiDurumu İadeİsteğiDurumu
        {
            get
            {
                return (İadeİsteğiDurumu)this.İadeİsteğiDurumuId;
            }
            set
            {
                this.İadeİsteğiDurumuId = (int)value;
            }
        }
        public virtual Kullanıcı Kullanıcı { get; set; }
    }
}
