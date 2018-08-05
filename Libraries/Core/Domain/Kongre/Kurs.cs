using System;

namespace Core.Domain.Kongre
{
    public partial class Kurs : TemelVarlık
    {
        public int KongreId { get; set; }
        public string KursAdı { get; set; }
        public string KursUcreti { get; set; }
        public DateTime KursBaslamaTarihi { get; set; }
        public DateTime KursBitisTarihi { get; set; }
        public string KursUcretiDoviz { get; set; }
        public string KursNotu { get; set; }
    }

}
