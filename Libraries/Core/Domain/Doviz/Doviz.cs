using System;

namespace Core.Domain.Doviz
{
    public partial class Doviz:TemelVarlık
    {
        public string Adı { get; set; }
        public string Kodu { get; set; }
        public decimal DovizAlış { get; set; }
        public decimal DovizSatış { get; set; }
        public decimal EfektifAlış { get; set; }
        public decimal EfektifSatış { get; set; }
        public DateTime Tarih { get; set; }
    }
}
