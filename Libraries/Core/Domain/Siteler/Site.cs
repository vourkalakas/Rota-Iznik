namespace Core.Domain.Siteler
{
    public partial class Site : TemelVarlık
    {
        public string Adı { get; set; }
        public string Url { get; set; }
        public bool SslEtkin { get; set; }
        public string GüvenliUrl { get; set; }
        public string Hosts { get; set; }
        public int VarsayılanDilId { get; set; }
        public int GörüntülemeSırası { get; set; }
        public string ŞirketAdı { get; set; }
        public string ŞirketAdresi { get; set; }
        public string ŞirketTelefon { get; set; }
    }
}
