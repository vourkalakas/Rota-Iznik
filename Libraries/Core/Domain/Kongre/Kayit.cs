namespace Core.Domain.Kongre
{
    public partial class Kayit : TemelVarlık
    {
        public int KongreId { get; set; }
        public string KayıtTipi { get; set; }
        public string KayıtUcreti { get; set; }
        public int KayıtUcretiDoviz { get; set; }
        public string DisKatilimciFarkı { get; set; }
        public string KayıtNotu { get; set; }
        public virtual Kongreler Kongre { get; set; }
    }
}
