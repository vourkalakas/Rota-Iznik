using Core;
using Core.Data;
using Core.Domain.Sayfalar;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Sayfalar
{
    public class SayfalarServisi : ISayfalarServisi
    {
        #region Constants

        private const string SAYFALAR_ALL_KEY = "sayfalar.all-{0}-{1}-{2}";
        private const string SAYFALAR_BY_ID_KEY = "sayfalar.id-{0}";
        private const string SAYFALAR_PATTERN_KEY = "sayfalar.";

        #endregion

        #region Fields

        private readonly IDepo<Sayfa> _sayfaDepo;
        //private readonly IDepo<SiteMapping> _storeMappingRepository;
        //private readonly ISiteMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        //private readonly IDepo<AclRecord> _aclRepository;
        //private readonly KatalogAyarları _katalogAyarları;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;

        #endregion

        #region Ctor

        public SayfalarServisi(IDepo<Sayfa> sayfaDepo,
            //IDepo<SiteMapping> storeMappingRepository,
            //IStoreMappingService storeMappingService,
            IWorkContext workContext,
            //IDepo<AclRecord> aclRepository,
            //KatalogAyarları katalogAyarları,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._sayfaDepo = sayfaDepo;
            //this._storeMappingRepository = storeMappingRepository;
            //this._storeMappingService = storeMappingService;
            this._workContext = workContext;
            //this._aclRepository = aclRepository;
            //this._katalogAyarları = katalogAyarları;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }

        #endregion

        #region Methods
        public virtual void SayfaSil(Sayfa sayfa)
        {
            if (sayfa == null)
                throw new ArgumentNullException("sayfa");

            _sayfaDepo.Sil(sayfa);
            _önbellekYönetici.KalıpİleSil(SAYFALAR_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(sayfa);
        }
        public virtual Sayfa SayfaAlId(int sayfaId)
        {
            if (sayfaId == 0)
                return null;

            string key = string.Format(SAYFALAR_BY_ID_KEY, sayfaId);
            return _önbellekYönetici.Al(key, () => _sayfaDepo.AlId(sayfaId));
        }
        public virtual Sayfa SayfaAlSistemAdı(string sistemAdı, int siteId = 0)
        {
            if (String.IsNullOrEmpty(sistemAdı))
                return null;

            var query = _sayfaDepo.Tablo;
            query = query.Where(t => t.SistemAdı == sistemAdı);
            query = query.OrderBy(t => t.Id);
            var topics = query.ToList();
            /*
            if (siteId > 0)
            {
                topics = topics.Where(x => _storeMappingService.Authorize(x, storeId)).ToList();
            }
            */
            return topics.FirstOrDefault();
        }
        public virtual IList<Sayfa> TümSayfalarıAl(int siteId, bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(SAYFALAR_ALL_KEY, siteId, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _sayfaDepo.Tablo;
                query = query.OrderBy(t => t.GörüntülenmeSırası).ThenBy(t => t.SistemAdı);

                if (!gizliOlanlarıGöster)
                    query = query.Where(t => t.Yayınlandı);
                /*
                if ((siteId > 0 && !_catalogSettings.IgnoreStoreLimitations) ||
                    (!ignorAcl && !_catalogSettings.IgnoreAcl))
                {
                    if (!ignorAcl && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "Topic" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations && storeId > 0)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Topic" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }
                    
                    query = from t in query
                            group t by t.Id
                            into tGroup
                            orderby tGroup.Key
                            select tGroup.FirstOrDefault();
                    query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
                }
                */
                return query.ToList();
            });
        }
        public virtual void SayfaEkle(Sayfa sayfa)
        {
            if (sayfa == null)
                throw new ArgumentNullException("sayfa");

            _sayfaDepo.Ekle(sayfa);
            _önbellekYönetici.KalıpİleSil(SAYFALAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(sayfa);
        }
        public virtual void SayfaGüncelle(Sayfa sayfa)
        {
            if (sayfa == null)
                throw new ArgumentNullException("sayfa");

            _sayfaDepo.Güncelle(sayfa);
            _önbellekYönetici.KalıpİleSil(SAYFALAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(sayfa);
        }

        #endregion
    }
}
