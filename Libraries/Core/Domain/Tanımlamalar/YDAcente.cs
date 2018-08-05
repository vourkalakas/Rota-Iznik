using System;
namespace Core.Domain.Tanımlamalar
{
    public partial class YDAcente : TemelVarlık
    {
        public string Adı { get; set; }
        public int Sektoru { get; set; }
        public int UlkeId { get; set; }
        public int SehirId { get; set; }
        public string Tel { get; set; }
        public string CepTel { get; set; }
        public string Adres { get; set; }
        public string Web { get; set; }
        public string Email { get; set; }
        public DateTime? OlusturulmaTarihi { get; set; }
    }
}
