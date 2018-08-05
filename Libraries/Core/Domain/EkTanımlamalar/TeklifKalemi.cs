namespace Core.Domain.EkTanımlamalar
{
    public partial class TeklifKalemi : TemelVarlık
    {
        public int? NodeId { get; set; }
        public string Adı { get; set; }
        public int SıraNo { get; set; }
        public int Kdv { get; set; }
        public int AnaBaslik { get; set; }
    }
}
