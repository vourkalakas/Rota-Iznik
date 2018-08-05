using System;

namespace Core.Domain.Görüşmeler
{
    public partial class GorusmeRaporlari : TemelVarlık
    {
        public string FirmaAdı { get; set; }
        public string GorusulenAdı { get; set; }
        public string GorusulenSoyadı { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Konu { get; set; }
        public DateTime Tarih { get; set; }
        public DateTime Deadline { get; set; }
        public bool Durumu { get; set; }
        public string Sorumlu { get; set; }
        public int Beklemede { get; set; }
        public int Olumsuz { get; set; }
        public int Takipte { get; set; }
        public int Teklif { get; set; }
        public int Grup { get; set; }
        public int GrupId { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }
}
