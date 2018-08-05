namespace Core.Domain.Genel
{
    public partial class GenelÖznitelik : TemelVarlık
    {
        public int VarlıkId { get; set; }
        public string KeyGroup { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int SiteId { get; set; }

    }
}
