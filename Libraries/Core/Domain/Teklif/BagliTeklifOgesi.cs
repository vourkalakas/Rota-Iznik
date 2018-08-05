namespace Core.Domain.Teklif
{
    public partial class BagliTeklifOgesi : TemelVarlık
    {
        public int TeklifId { get; set; }
        public string Adı { get; set; }
        public string Aciklama { get; set; }
        public decimal AlisBirimFiyat { get; set; }
        public decimal AlisBirimFiyatDolar { get; set; }
        public decimal AlisBirimFiyatEuro { get; set; }
        public decimal SatisBirimFiyat { get; set; }
        public decimal SatisBirimFiyatDolar { get; set; }
        public decimal SatisBirimFiyatEuro { get; set; }
        public decimal AlisFiyat { get; set; }
        public decimal AlisFiyatDolar { get; set; }
        public decimal AlisFiyatEuro { get; set; }
        public decimal SatisFiyat { get; set; }
        public decimal SatisFiyatDolar { get; set; }
        public decimal SatisFiyatEuro { get; set; }
        public decimal ToplamFiyat { get; set; }
        public decimal ToplamFiyatDolar { get; set; }
        public decimal ToplamFiyatEuro { get; set; }
        public decimal Kar { get; set; }
        public decimal KarDolar { get; set; }
        public decimal KarEuro { get; set; }
        public string Gelir { get; set; }
        public int Adet { get; set; }
        public int Gun { get; set; }
        public int Kdv { get; set; }
        public int Parabirimi { get; set; }
        public int Vparent { get; set; }
        public string Tparent { get; set; }
        public string Kurum { get; set; }
        public int AlisAdet { get; set; }
    }
}
