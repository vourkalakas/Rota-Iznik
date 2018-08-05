namespace Core.Domain.Konum
{
    public partial class Ilce:TemelVarlık
    {
        public string Adı { get; set; }
        public int SehirId { get; set; }
        public int UlkeId { get; set; }
    }
}
