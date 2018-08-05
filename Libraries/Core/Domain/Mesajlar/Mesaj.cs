using System;

namespace Core.Domain.Mesajlar
{
    public partial class Mesaj : TemelVarlık
    {
        public int KullanıcıId { get; set; }
        public string KullanıcıAdı { get; set; }
        public int GonderenId { get; set; }
        public string GonderenAdı { get; set; }
        public string Baslik { get; set; }
        public string Msj { get; set; }
        public bool Bildirim { get; set; }
        public bool Okundu { get; set; }
        public DateTime OlusmaTarihi { get; set; }
    }

}
