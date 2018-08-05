using System;

namespace Core.Domain.Teklif
{
    public partial class Teklif : TemelVarlık
    {
        public string Adı { get; set; }
        public int HazırlayanId { get; set; }
        public int SunanId { get; set; }
        public int Operasyon { get; set; }
        public int Konfirme { get; set; }
        public int Biten { get; set; }
        public int Iptal { get; set; }
        public string Aciklama { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public DateTime OpsiyonTarihi { get; set; }
        public int UlkeId { get; set; }
        public int SehirId { get; set; }
        public int HizmetBedeli { get; set; }
        public int FirmaId { get; set; }
        public int YetkiliId { get; set; }
        public string Konum { get; set; }
        public string Kod { get; set; }
        public decimal KurDolar { get; set; }
        public decimal KurEuro { get; set; }
        public string MekanAdı { get; set; }
        public string ToplantıAdı { get; set; }
        public int OrijinalTeklifId { get; set; }
        public string Durumu { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }
}
