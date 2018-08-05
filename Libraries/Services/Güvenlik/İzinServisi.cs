using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Önbellek;
using Core.Data;
using Core.Domain.Kullanıcılar;
using Core.Domain.Güvenlik;
using Services.Kullanıcılar;

namespace Services.Güvenlik
{
    public partial class İzinServisi : IİzinServisi
    {
        #region Constants
        private const string IZINLER_IZIN_ANAHTARI = "izin.izinli-{0}-{1}";
        private const string IZINLER_DOKU_ANAHTARI = "izin.";
        #endregion

        #region Fields

        private readonly IDepo<İzinKaydı> _izinKaydıDeposu;
        private readonly IKullanıcıServisi _kullanıcıServisi;
        private readonly IWorkContext _workContext;
        private readonly IÖnbellekYönetici _önbellekYönetici;

        #endregion

        #region Ctor
        
        public İzinServisi(IDepo<İzinKaydı> izinKaydıDeposu,
            IKullanıcıServisi kullanıcıServisi,
            IWorkContext workContext,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._izinKaydıDeposu = izinKaydıDeposu;
            this._kullanıcıServisi = kullanıcıServisi;
            this._workContext = workContext;
            this._önbellekYönetici = önbellekYönetici;
        }

        #endregion

        #region Utilities
        protected virtual bool YetkiVer(string izinKaydıSistemAdı, KullanıcıRolü kullanıcıRolü)
        {
            if (String.IsNullOrEmpty(izinKaydıSistemAdı))
                return false;

            string key = string.Format(IZINLER_IZIN_ANAHTARI, kullanıcıRolü.Id, izinKaydıSistemAdı);
            return _önbellekYönetici.Al(key, () =>
            {
                foreach (var izin1 in kullanıcıRolü.İzinKayıtları)
                    if (izin1.SistemAdı.Equals(izinKaydıSistemAdı, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        #endregion

        #region Methods
        public virtual void İzinKaydınıSil(İzinKaydı izin)
        {
            if (izin == null)
                throw new ArgumentNullException("izin");

            _izinKaydıDeposu.Sil(izin);

            _önbellekYönetici.KalıpİleSil(IZINLER_DOKU_ANAHTARI);
        }
        public virtual İzinKaydı İzinKaydıAlId(int izinId)
        {
            if (izinId == 0)
                return null;

            return _izinKaydıDeposu.AlId(izinId);
        }
        public virtual İzinKaydı İzinKaydıAlSistemAdı(string sistemAdı)
        {
            if (String.IsNullOrWhiteSpace(sistemAdı))
                return null;

            var sorgu = from pr in _izinKaydıDeposu.Tablo
                        where pr.SistemAdı == sistemAdı
                        orderby pr.Id
                        select pr;

            var izinKaydı = sorgu.FirstOrDefault();
            return izinKaydı;
        }
        public virtual IList<İzinKaydı> TümİzinKayıtlarınıAl()
        {
            var sorgu = from pr in _izinKaydıDeposu.Tablo
                        orderby pr.Adı
                        select pr;
            var izinler = sorgu.ToList();
            return izinler;
        }
        public virtual void İzinKaydıEkle(İzinKaydı izin)
        {
            if (izin == null)
                throw new ArgumentNullException("izin");

            _izinKaydıDeposu.Ekle(izin);

            _önbellekYönetici.KalıpİleSil(IZINLER_DOKU_ANAHTARI);
        }
        public virtual void İzinKaydıGüncelle(İzinKaydı izin)
        {
            if (izin == null)
                throw new ArgumentNullException("izin");

            _izinKaydıDeposu.Güncelle(izin);

            _önbellekYönetici.KalıpİleSil(IZINLER_DOKU_ANAHTARI);
        }
        public virtual void İzinleriKur(IİzinSağlayıcı izinSağlayıcı)
        {
            //yeni izinleri kur
            var izinler = izinSağlayıcı.İzinleriAl();
            foreach (var izin in izinler)
            {
                var izin1 = İzinKaydıAlSistemAdı(izin.SistemAdı);
                if (izin1 == null)
                {
                    //yeni izin (kur)
                    izin1 = new İzinKaydı
                    {
                        Adı = izin.Adı,
                        SistemAdı = izin.SistemAdı,
                        Kategori = izin.Kategori,
                    };


                    //varsayılan kullanıcı rol mapping
                    var varsayılanİzinler = izinSağlayıcı.VarsayılanİzinleriAl();
                    foreach (var varsayılanİzin in varsayılanİzinler)
                    {
                        var kullanıcıRolü = _kullanıcıServisi.KullanıcıRolüAlSistemAdı(varsayılanİzin.KullanıcıRolüSistemAdı);
                        if (kullanıcıRolü == null)
                        {
                            //yeni rol (kaydet)
                            kullanıcıRolü = new KullanıcıRolü
                            {
                                Adı = varsayılanİzin.KullanıcıRolüSistemAdı,
                                Aktif = true,
                                SistemAdı = varsayılanİzin.KullanıcıRolüSistemAdı
                            };
                            _kullanıcıServisi.KullanıcıRolüEkle(kullanıcıRolü);
                        }


                        var varsayılanMappingSağlandı = (from p in varsayılanİzin.İzinKayıtları
                                                      where p.SistemAdı == izin1.SistemAdı
                                                      select p).Any();
                        var mappingMevcut = (from p in kullanıcıRolü.İzinKayıtları
                                             where p.SistemAdı == izin1.SistemAdı
                                             select p).Any();
                        if (varsayılanMappingSağlandı && !mappingMevcut)
                        {
                            izin1.KullanıcıRolleri.Add(kullanıcıRolü);
                        }
                    }

                    //yeni izini kaydet
                    İzinKaydıEkle(izin1);
                }
            }
        }
        public virtual void İzinleriKaldır(IİzinSağlayıcı permissionProvider)
        {
            var izinler = permissionProvider.İzinleriAl();
            foreach (var izin in izinler)
            {
                var izin1 = İzinKaydıAlSistemAdı(izin.SistemAdı);
                if (izin1 != null)
                {
                    İzinKaydınıSil(izin1);
                }
            }

        }
        public virtual bool YetkiVer(İzinKaydı izin)
        {
            return YetkiVer(izin, _workContext.MevcutKullanıcı);
        }
        public virtual bool YetkiVer(İzinKaydı izin, Kullanıcı kullanıcı)
        {
            if (izin == null)
                return false;

            if (kullanıcı == null)
                return false;

            return YetkiVer(izin.SistemAdı, kullanıcı);
        }
        public virtual bool YetkiVer(string izinKaydıSistemAdı)
        {
            return YetkiVer(izinKaydıSistemAdı, _workContext.MevcutKullanıcı);
        }
        public virtual bool YetkiVer(string izinKaydıSistemAdı, Kullanıcı kullanıcı)
        {
            if (String.IsNullOrEmpty(izinKaydıSistemAdı))
                return false;

            var kullanıcıRolleri = kullanıcı.KullanıcıRolleri.Where(cr => cr.Aktif);
            foreach (var rol in kullanıcıRolleri)
                if (YetkiVer(izinKaydıSistemAdı, rol))
                    return true;
            return false;
        }

        #endregion
    }
}
