namespace Core.Domain.Siteler
{
    public partial class SiteMapping : TemelVarlık
    {
        public int VarlıkId { get; set; }

        public string VarlıkAdı { get; set; }

        public int SiteId { get; set; }

        public virtual Site Site { get; set; }
    }
}
