using System;
namespace Core.Domain.Tanımlamalar
{
    public partial class Musteri : TemelVarlık
    {
        public string Adı { get; set; }
        public int Sektoru { get; set; }
        public int Kategori { get; set; }
        public string Tel { get; set; }
        public string Faks { get; set; }
        public string Sehir { get; set; }
        public int SehirId { get; set; }
        public string Ilce { get; set; }
        public int IlceId { get; set; }
        public string Adres { get; set; }
        public string Web { get; set; }
        public string Email { get; set; }
        public DateTime? OlusturulmaTarihi { get; set; }
    }
}
