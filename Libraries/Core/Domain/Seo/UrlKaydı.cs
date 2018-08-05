namespace Core.Domain.Seo
{
    public partial class UrlKaydı : TemelVarlık
    {
        public int VarlıkId { get; set; }

        public string VarlıkAdı { get; set; }

        public string Slug { get; set; }

        public bool Aktif { get; set; }
    }
}
