namespace Core.Domain.Kongre
{
    public partial class Kontenjan : TemelVarlık
    {
        public int KongreId { get; set; }
        public int OtelId { get; set; }
        public string OtelKonaklamaTipi { get; set; }
        public int OdaKisiSayısı { get; set; }
        public int OtelKontenjanı { get; set; }

    }
}
