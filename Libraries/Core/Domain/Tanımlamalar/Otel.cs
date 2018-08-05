using System;

namespace Core.Domain.Tanımlamalar
{
    public partial class Otel : TemelVarlık
    {
        public string Adı { get; set; }
        public string Unvanı { get; set; }
        public string Tel { get; set; }
        public string Faks { get; set; }
        public int SehirId { get; set; }
        public int IlceId { get; set; }
        public string Adres { get; set; }
        public string Web { get; set; }
        public string Email { get; set; }
        public int CalismaSekli { get; set; }
        public DateTime? OlusturulmaTarihi { get; set; }
    }
}
