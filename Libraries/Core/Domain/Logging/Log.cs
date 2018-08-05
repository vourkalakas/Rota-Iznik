using System;
using Core.Domain.Kullanıcılar;

namespace Core.Domain.Logging
{
    public partial class Log : TemelVarlık
    {
        public int LogSeviyeId { get; set; }
        public string KısaMesaj { get; set; }
        public string TamMesaj { get; set; }
        public string IpAdresi { get; set; }
        public int? KullamıcıId { get; set; }
        public string SayfaUrl { get; set; }
        public string YönlendirenURL { get; set; }
        public DateTime OluşturulmaTarihi { get; set; }
        public LogSeviyesi LogSeviyesi
        {
            get
            {
                return (LogSeviyesi)this.LogSeviyeId;
            }
            set
            {
                this.LogSeviyeId = (int)value;
            }
        }
        public virtual Kullanıcı Kullanıcı { get; set; }
    }
}
