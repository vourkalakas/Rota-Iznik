using System;

namespace Core.Domain.Finans
{
    public partial class OdemeFormu : TemelVarlık
    {
        public int Bolum { get; set; }
        public int BolumNo { get; set; }
        public string Firma { get; set; }
        public int Banka { get; set; }
        public string SubeKodu { get; set; }
        public string HesapNo { get; set; }
        public string IBAN { get; set; }
        public decimal Tutar { get; set; }
        public int ParaBirimi { get; set; }
        public int OdemeSekli { get; set; }
        public int OdemeTuru { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public string Aciklama { get; set; }
        public int Ilgili { get; set; }
        public int Onay1 { get; set; }
        public int Onay2 { get; set; }
        public int Onay3 { get; set; }
        public string PO { get; set; }
        public DateTime KongreTarihi { get; set; }
        public string KongreAdı { get; set; }
        public string FaturaNo { get; set; }
        public string SatisFaturaNo { get; set; }
        public string TutarGrup { get; set; }
        public string KalemGrup { get; set; }
        public bool Onay { get; set; }
    }

}
