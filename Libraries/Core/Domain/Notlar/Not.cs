using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Notlar
{
    public partial class Not:TemelVarlık
    {
        public int KullanıcıId { get; set; }
        public int GrupId { get; set; }
        public string Grup { get; set; }
        public string Icerik { get; set; }
    }
}
