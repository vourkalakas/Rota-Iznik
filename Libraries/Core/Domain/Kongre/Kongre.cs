using System;

namespace Core.Domain.Kongre
{
    public partial class Kongreler : TemelVarlık
    {
        public string Adı { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal KurDolar { get; set; }
        public decimal KurEuro { get; set; }
    }
}
