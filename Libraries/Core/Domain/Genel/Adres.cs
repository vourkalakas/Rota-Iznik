using System;

namespace Core.Domain.Genel
{
    public partial class Adres : TemelVarlık, ICloneable
    {
        public string Adı { get; set; }
        public string Soyadı { get; set; }
        public string Email { get; set; }
        public string Şirket { get; set; }
        public int? ÜlkeId { get; set; }
        public string Şehir { get; set; }
        public string Adres1 { get; set; }
        public string Adres2 { get; set; }
        public string PostaKodu { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string ÖzelÖznitelik { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }

        public object Clone()
        {
            var addr = new Adres
            {
                Adı = this.Adı,
                Soyadı = this.Soyadı,
                Email = this.Email,
                Şirket = this.Şirket,
                ÜlkeId = this.ÜlkeId,
                Şehir = this.Şehir,
                Adres1 = this.Adres1,
                Adres2 = this.Adres2,
                PostaKodu = this.PostaKodu,
                Tel = this.Tel,
                Fax = this.Fax,
                ÖzelÖznitelik = this.ÖzelÖznitelik,
                OluşturulmaTarihi = this.OluşturulmaTarihi,
            };
            return addr;
        }
    }
}
