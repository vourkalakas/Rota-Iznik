using System;
using System.Collections.Generic;
using Core.Domain.Genel;
using Core.Domain.Siparişler;

namespace Core.Domain.Kullanıcılar
{
    public partial class Kullanıcı : TemelVarlık
    {
        private ICollection<HariciKimlikDoğrulamaKaydı> _hariciKimlikDoğrulamaKayıtları;
        private ICollection<KullanıcıRolü> _kullanıcıRolleri;
        private ICollection<İadeİsteği> _iadeİsteği;
        private ICollection<Adres> _adres;

        public Kullanıcı()
        {
            this.KullanıcıGuid = Guid.NewGuid();
        }
        public Guid KullanıcıGuid { get; set; }
        public string KullanıcıAdı { get; set; }
        public string Email { get; set; }
        public string EmailDoğrulandı { get; set; }
        public string YöneticiYorumu { get; set; }
        public bool VergidenMuaf { get; set; }
        public int SatıcıId { get; set; }
        public bool SepetiDolu { get; set; }
        public bool GirişGerekli { get; set; }
        public int HatalıGirişSayısı { get; set; }
        public DateTime? ŞuTarihdenBeriGirişYapamıyor { get; set; }
        public bool Aktif { get; set; }
        public bool Silindi { get; set; }
        public bool SistemHesabı { get; set; }
        public string SistemAdı { get; set; }
        public string SonIPAdresi { get; set; }
        public DateTime ŞuTarihdeOluşturuldu { get; set; }
        public DateTime? SonGirişTarihi { get; set; }
        public DateTime SonİşlemTarihi { get; set; }
        public int KayıtOlduSiteId { get; set; }
        public string AdminYorumu { get; set; }

        #region Navigation properties
        public virtual ICollection<HariciKimlikDoğrulamaKaydı> HariciKimlikDoğrulamaKayıtları
        {
            get { return _hariciKimlikDoğrulamaKayıtları ?? (_hariciKimlikDoğrulamaKayıtları = new List<HariciKimlikDoğrulamaKaydı>()); }
            protected set { _hariciKimlikDoğrulamaKayıtları = value; }
        }
        public virtual ICollection<KullanıcıRolü> KullanıcıRolleri
        {
            get { return _kullanıcıRolleri ?? (_kullanıcıRolleri = new List<KullanıcıRolü>()); }
            protected set { _kullanıcıRolleri = value; }
        }
        public virtual ICollection<İadeİsteği> İadeİstekleri
        {
            get { return _iadeİsteği ?? (_iadeİsteği = new List<İadeİsteği>()); }
            protected set { _iadeİsteği = value; }
        }
        public virtual Adres FaturaAdresi { get; set; }
        public virtual ICollection<Adres> Adresler
        {
            get { return _adres ?? (_adres = new List<Adres>()); }
            protected set { Adresler = value; }
        }

        #endregion
    }
}
