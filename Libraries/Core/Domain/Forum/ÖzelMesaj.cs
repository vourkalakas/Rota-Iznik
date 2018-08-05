using Core.Domain.Kullanıcılar;
using System;

namespace Core.Domain.Forum
{
    public partial class ÖzelMesaj : TemelVarlık
    {
        public int SiteId { get; set; }
        public int KullanıcıdanId { get; set; }
        public int KullanıcıyaId { get; set; }
        public string Konu { get; set; }
        public string Mesaj { get; set; }
        public bool Okundu { get; set; }
        public bool YazarTarafındanSilindi { get; set; }
        public bool AlıcıTarafındanSilindi { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public virtual Kullanıcı Kullanıcıdan { get; set; }
        public virtual Kullanıcı Kullanıcıya { get; set; }
    }
}
