namespace Core.Domain.Sayfalar
{
    public partial class SayfaTema : TemelVarlık
    {
        public string Adı { get; set; }
        public string Yolu { get; set; }
        public int GörüntülenmeSırası { get; set; }
    }
}
