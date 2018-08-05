using System.Collections.Generic;
using Core.Domain.Güvenlik;

namespace Core.Domain.Kullanıcılar
{
    public partial class KullanıcıRolü : TemelVarlık
    {
        private ICollection<İzinKaydı> _izinKayıtları;
        public string Adı { get; set; }
        public bool Aktif { get; set; }
        public bool SistemRolü { get; set; }
        public string SistemAdı { get; set; }
        public bool ParolaÖmrünüEtkinleştir { get; set; }
        public virtual ICollection<İzinKaydı> İzinKayıtları
        {
            get { return _izinKayıtları ?? (_izinKayıtları = new List<İzinKaydı>()); }
            protected set { _izinKayıtları = value; }
        }
    }
}