using System;

namespace Core.Domain.Kongre
{
    public partial class Katilimci : TemelVarlık
    {
        public int KongreId { get; set; }
        public string Adı { get; set; }
        public string Soyadı { get; set; }
        public string TCKN { get; set; }
        public int UlkeId { get; set; }
        public int SehirId { get; set; }
        public int IlceId { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public int Refakatci { get; set; }
        public bool KayıtOldu { get; set; }
        public int KayıtId { get; set; }
        public int KayıtSponsorId { get; set; }
        public int KursId { get; set; }
        public int KursSponsorId { get; set; }
        public int KonaklamaId { get; set; }
        public int KonaklamaSponsorId { get; set; }
        public DateTime OtelGiris { get; set; }
        public DateTime OtelCikis { get; set; }
        public int TransferId { get; set; }
        public int TransferSponsorId { get; set; }
        public string UlasimUcreti { get; set; }
        public int UlasimUcretiDoviz { get; set; }
        public string UlasimKalkisParkur { get; set; }
        public string UlasimVarisParkur { get; set; }
        public string UlasimKalkisUcus { get; set; }
        public string UlasimVarisUcus { get; set; }
        public DateTime UlasimVarisTarihi { get; set; }
        public DateTime UlasimKalkisTarihi { get; set; }
        public DateTime TransferTarihi { get; set; }
        public bool Iptal { get; set; }
        
    }

}
