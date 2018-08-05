using System;

namespace Core.Domain.Kongre
{
    public partial class Konaklama : TemelVarlık
    {
        public int KongreId { get; set; }
        public string KonaklamaAdı { get; set; }
        public string OtelAdi { get; set; }
        public string OtelUcreti { get; set; }
        public string OtelUcretiDoviz { get; set; }
        public DateTime? OtelGiris { get; set; }
        public DateTime? OtelCikis { get; set; }
        public string OdaTipi { get; set; }
        public string GecelikFark { get; set; }
        public string OtelNotu { get; set; }
        public int OtelKontenjanı { get; set; }
    }

}
