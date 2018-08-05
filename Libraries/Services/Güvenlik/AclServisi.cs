using System;
using System.Collections.Generic;
using Core;
using Core.Domain.Güvenlik;
using Core.Domain.Kullanıcılar;
using Core.Data;
using Core.Önbellek;
using Services.Olaylar;
using Core.Domain.Katalog;
using System.Linq;

namespace Services.Güvenlik
{
    public partial class AclServisi : IAclServisi
    {
        private const string ACLKAYDI_BY_ENTITYID_NAME_KEY = "aclrecord.entityid-name-{0}-{1}";
        private const string ACLKAYDI_PATTERN_KEY = "aclrecord.";
        private readonly IDepo<AclKaydı> _aclDepo;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IWorkContext _workContext;
        private readonly KatalogAyarları _katalogAyarları;
        public AclServisi(IDepo<AclKaydı> aclDepo,
            IÖnbellekYönetici önbellekYönetici,
            IOlayYayınlayıcı olayYayınlayıcı,
            IWorkContext workContext,
            KatalogAyarları katalogAyarları)
        {
            this._aclDepo = aclDepo;
            this._önbellekYönetici = önbellekYönetici;
            this._workContext = workContext;
            this._katalogAyarları = katalogAyarları;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }
        public virtual AclKaydı AclKaydıAlId(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;
            return _aclDepo.AlId(aclRecordId);
        }

        public virtual void AclKaydıEkle(AclKaydı aclKaydı)
        {
            if (aclKaydı == null)
                throw new ArgumentNullException("aclKaydı");
            _aclDepo.Ekle(aclKaydı);
            _önbellekYönetici.KalıpİleSil(ACLKAYDI_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(aclKaydı);
        }

        public virtual void AclKaydıEkle<T>(T varlık, int kullanıcıRolId) where T : TemelVarlık, IAclDestekli
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            if (kullanıcıRolId == 0)
                throw new ArgumentOutOfRangeException("kullanıcıRolId");

            int varlıkId = varlık.Id;
            string varlıkAdı = typeof(T).Name;

            var aclKaydı = new AclKaydı
            {
                VarlıkId = varlıkId,
                VarlıkAdı = varlıkAdı,
                KullanıcıRolId = kullanıcıRolId
            };
            AclKaydıEkle(aclKaydı);
        }

        public virtual void AclKaydıGüncelle(AclKaydı aclKaydı)
        {
            if (aclKaydı == null)
                throw new ArgumentNullException("aclKaydı");
            _aclDepo.Güncelle(aclKaydı);
            _önbellekYönetici.KalıpİleSil(ACLKAYDI_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(aclKaydı);
        }

        public virtual void AclKaydıSil(AclKaydı aclKaydı)
        {
            if (aclKaydı == null)
                throw new ArgumentNullException("aclKaydı");
            _aclDepo.Sil(aclKaydı);
            _önbellekYönetici.KalıpİleSil(ACLKAYDI_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(aclKaydı);
        }

        public virtual int[] ErişimİleKullanıcıRolIdleriAl<T>(T varlık) where T : TemelVarlık, IAclDestekli
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            int varlıkId = varlık.Id;
            string varlıkAdı = typeof(T).Name;

            string key = string.Format(ACLKAYDI_BY_ENTITYID_NAME_KEY, varlıkId, varlıkAdı);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = from ur in _aclDepo.Tablo
                            where ur.VarlıkId == varlıkId &&
                            ur.VarlıkAdı == varlıkAdı
                            select ur.KullanıcıRolId;
                return sorgu.ToArray();
            });
        }

        public virtual IList<AclKaydı> TümAclKayıtları<T>(T varlık) where T : TemelVarlık, IAclDestekli
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            int varlıkId = varlık.Id;
            string varlıkAdı = typeof(T).Name;

            var sorgu = from ur in _aclDepo.Tablo
                        where ur.VarlıkId == varlıkId &&
                        ur.VarlıkAdı == varlıkAdı
                        select ur;
            var aclKayıtları = sorgu.ToList();
            return aclKayıtları;
        }

        public virtual bool YetkiVer<T>(T varlık) where T : TemelVarlık, IAclDestekli
        {
            return YetkiVer(varlık, _workContext.MevcutKullanıcı);
        }

        public virtual bool YetkiVer<T>(T varlık, Kullanıcı kullanıcı) where T : TemelVarlık, IAclDestekli
        {
            if (varlık == null)
                return false;

            if (kullanıcı == null)
                return false;

            if (_katalogAyarları.IgnoreAcl)
                return true;

            if (!varlık.AclKonusu)
                return true;

            foreach (var role1 in kullanıcı.KullanıcıRolleri.Where(cr => cr.Aktif))
                foreach (var role2Id in ErişimİleKullanıcıRolIdleriAl(varlık))
                    if (role1.Id == role2Id)
                        return true;
            return false;
        }
    }
}
