using System;

namespace Core.Domain.Yetkililer
{
    public partial class Yetkili : TemelVarlık
    {
        public int Grup { get; set; }
        public int GrupId { get; set; }
        public string Unvan { get; set; }
        public string Adı { get; set; }
        public string Soyadı { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
    }
}
