using System;

namespace Core.Domain.Mesajlar
{
    public partial class EmailHesabı : TemelVarlık
    {
        public string Email { get; set; }
        public string GörüntülenenAd { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string KullanıcıAdı { get; set; }
        public string Şifre { get; set; }
        public bool SslEtkin { get; set; }
        public bool VarsayılanKimlikBilgileriniKullan { get; set; }
        public string KısaAdı
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.GörüntülenenAd))
                    return this.Email + " (" + this.GörüntülenenAd + ")";
                return this.Email;
            }
        }
    }
}
