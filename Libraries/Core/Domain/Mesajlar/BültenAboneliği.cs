using System;

namespace Core.Domain.Mesajlar
{
    public partial class BültenAboneliği:TemelVarlık
    {
        public Guid BültenAboneliğiGuid { get; set; }
        public string Email { get; set; }
        public bool Aktif { get; set; }
        public int SiteId { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
    }
}

