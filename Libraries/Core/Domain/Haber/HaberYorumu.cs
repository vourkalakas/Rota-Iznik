using Core.Domain.Kullanıcılar;
using Core.Domain.Siteler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Haber
{
    public partial class HaberYorumu : TemelVarlık
    {
        public string YorumBaşlığı { get; set; }
        public string YorumYazısı { get; set; }
        public int HaberÖğesiId { get; set; }
        public int KullanıcıId { get; set; }
        public bool Onaylandı { get; set; }
        public int SiteId { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public virtual Kullanıcı Kullanıcı { get; set; }
        public virtual HaberÖğesi HaberÖğesi { get; set; }
        public virtual Site Site { get; set; }
    }
}
