using System;

namespace Core.Domain.Teklif
{
    public partial class TeklifHarici : TemelVarlık
    {
        public string Adı { get; set; }
        public string Po { get; set; }
        public int BelgeTuru { get; set; }
        public int HazırlayanId { get; set; }
        public DateTime Tarih { get; set; }
        public DateTime TeslimTarihi { get; set; }
        public int UlkeId { get; set; }
        public int SehirId { get; set; }
        public string Acenta { get; set; }
        public string TalepNo { get; set; }
        public decimal HizmetBedeli { get; set; }
        public int Parabirimi { get; set; }
        public int Fatura { get; set; }
        public string FaturaNo { get; set; }
        public bool Onay { get; set; }
    }
}
