using Core.Domain.Siteler;

namespace Core.Domain.Mesajlar
{
    public partial class MesajTeması : TemelVarlık,ISiteMappingDestekli
    {
        public string Adı { get; set; }
        public string BccEmailAdresleri { get; set; }
        public string Konu { get; set; }
        public string Gövde { get; set; }
        public bool Aktif { get; set; }
        public int? GöndermedenÖnceGeciktir { get; set; }
        public int GeciktirmePeriodId { get; set; }
        public int EkİndirmeId { get; set; }
        public int EmailHesapId { get; set; }
        public bool SitelerdeSınırlı { get; set; }
        public MesajGecikmePeriodu GecikmePeriodu
        {
            get { return (MesajGecikmePeriodu)this.GeciktirmePeriodId; }
            set { this.GeciktirmePeriodId = (int)value; }
        }
    }
}
