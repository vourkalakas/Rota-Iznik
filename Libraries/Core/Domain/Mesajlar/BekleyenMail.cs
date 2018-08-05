using System;

namespace Core.Domain.Mesajlar
{

    public partial class BekleyenMail : TemelVarlık
    {
        public int ÖncelikId { get; set; }
        public string Kimden { get; set; }
        public string KimdenAd { get; set; }
        public string Kime { get; set; }
        public string KimeAd { get; set; }
        public string Yanıtla { get; set; }
        public string YanıtlaAd { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string Konu { get; set; }
        public string Gövde { get; set; }
        public string EkDosyaYolu { get; set; }
        public string EkDosyaAdı { get; set; }
        public int EkYüklemeId { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public DateTime? ŞuTarihdenÖnceGönderme { get; set; }
        public int GöndermeDenemesi { get; set; }
        public DateTime? TarihindeGönderildi { get; set; }
        public int EmailHesapId { get; set; }
        public virtual EmailHesabı EmailHesabı { get; set; }
        public BekleyenMailÖnceliği Öncelik
        {
            get
            {
                return (BekleyenMailÖnceliği)this.ÖncelikId;
            }
            set
            {
                this.ÖncelikId = (int)value;
            }
        }

    }
}
