using System;

namespace Core.Domain.Görevler
{
    public partial class ZamanlanmışGörevler : TemelVarlık
    {
        public string Adı { get; set; }
        public int Saniyeler { get; set; }
        public string Tipi { get; set; }
        public bool Etkin { get; set; }
        public bool HatadaDurdur { get; set; }
        public DateTime? SonBaşlamaTarihi { get; set; }
        public DateTime? SonBitişTarihi { get; set; }
        public DateTime? SonBaşarılıTarihi { get; set; }
    }
}
