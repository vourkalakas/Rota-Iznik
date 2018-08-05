using Core;
using Core.Data;
using Core.Domain.Genel;
using Core.Domain.Katalog;
using Core.Önbellek;
using Data;
using Services.Kullanıcılar;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Katalog
{
    public partial class KategoriServisi:IKategoriServisi
    {
        private const string KATEGORILER_BY_ID_KEY = "kategori.id-{0}";
        private const string KATEGORILER_BY_PARENT_KATEGORI_ID_KEY = "kategori.byparent-{0}-{1}-{2}-{3}-{4}";
        private const string KATEGORILER_PATTERN_KEY = "kategori.";

        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly GenelAyarlar _genelAyarlar;
        private readonly IDataSağlayıcı _dataSağlayıcı;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IDepo<Kategori> _kategoriDepo;
        private readonly ISiteContext _siteContext;
        private readonly IÖnbellekYönetici _önbellekYönetici;

        public KategoriServisi(IOlayYayınlayıcı olayYayınlayıcı,
            GenelAyarlar genelAyarlar,
            IDataSağlayıcı dataSağlayıcı,
            IWorkContext workContext,
            IDbContext dbContext,
            IDepo<Kategori> kategoriDepo,
            ISiteContext siteContext,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._genelAyarlar = genelAyarlar;
            this._dataSağlayıcı = dataSağlayıcı;
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._kategoriDepo = kategoriDepo;
            this._siteContext = siteContext;
            this._önbellekYönetici = önbellekYönetici;
        }
        public virtual void KategoriSil(Kategori kategori)
        {
            if (kategori == null)
                throw new ArgumentNullException("kategori");
            kategori.Silindi = true;
            KategoriGüncelle(kategori);
            _olayYayınlayıcı.OlaySilindi(kategori);
            var altKategoriler = TümKategorileriAlParentKategoriId(kategori.Id, true);
            foreach (var altKategori in altKategoriler)
            {
                altKategori.ParentKategoriId = 0;
                KategoriGüncelle(altKategori);
            }
        }
        public virtual ISayfalıListe<Kategori> TümKategorileriAl(string kategoriAdı="",int SiteId=0,
            int pageIndex=0,int pageSize=int.MaxValue,bool GizliOlanlarıGöster = false)
        {
            if(_genelAyarlar.StoredProcedureKullanYüklüKategoriler&&
                _genelAyarlar.StoredProcedureKullanDestekliyse&&_dataSağlayıcı.StoredProceduredDestekli)
            {
                var gizliOlanlarıGösterParametresi = _dataSağlayıcı.ParametreAl();
                gizliOlanlarıGösterParametresi.ParameterName = "GizliOlanlarıGöster";
                gizliOlanlarıGösterParametresi.Value = GizliOlanlarıGöster;
                gizliOlanlarıGösterParametresi.DbType = DbType.Boolean;

                var AdıParametresi = _dataSağlayıcı.ParametreAl();
                AdıParametresi.ParameterName = "Adı";
                AdıParametresi.Value = kategoriAdı;
                AdıParametresi.DbType = DbType.String;

                var siteIdParametresi = _dataSağlayıcı.ParametreAl();
                siteIdParametresi.ParameterName = "SiteId";
                siteIdParametresi.Value = SiteId;
                siteIdParametresi.DbType = DbType.Int32;

                var KullanıcıRolIdParametresi = _dataSağlayıcı.ParametreAl();
                KullanıcıRolIdParametresi.ParameterName = "KullanıcıRolleri";
                KullanıcıRolIdParametresi.Value = string.Join(",", _workContext.MevcutKullanıcı.KullanıcıRolIdleri());
                KullanıcıRolIdParametresi.DbType = DbType.String;

                var PageIndexParametresi = _dataSağlayıcı.ParametreAl();
                PageIndexParametresi.ParameterName = "PageIndex";
                PageIndexParametresi.Value = pageIndex;
                PageIndexParametresi.DbType = DbType.Int32;

                var PageSizeParametresi = _dataSağlayıcı.ParametreAl();
                PageSizeParametresi.ParameterName = "PageSize";
                PageSizeParametresi.Value = pageSize;
                PageSizeParametresi.DbType = DbType.Int32;

                var ToplamKayıtParametresi = _dataSağlayıcı.ParametreAl();
                ToplamKayıtParametresi.ParameterName = "ToplamKayıt";
                ToplamKayıtParametresi.Value = ParameterDirection.Output;
                ToplamKayıtParametresi.DbType = DbType.Int32;

                var kategoriler = _dbContext.StoredProcedureListesiniÇalıştır<Kategori>("TümSayfalananKategoriYükle", gizliOlanlarıGösterParametresi,
                    AdıParametresi, siteIdParametresi, KullanıcıRolIdParametresi, PageIndexParametresi, PageSizeParametresi, ToplamKayıtParametresi);
                var toplamKayıt = (ToplamKayıtParametresi.Value != DBNull.Value) ? Convert.ToInt32(ToplamKayıtParametresi.Value) : 0;
                return new SayfalıListe<Kategori>(kategoriler, pageIndex, pageSize, toplamKayıt);
            }
            else
            {
                var sorgu = _kategoriDepo.Tablo;
                if (!GizliOlanlarıGöster)
                    sorgu = sorgu.Where(c => c.Yayınlandı);
                if (!String.IsNullOrWhiteSpace(kategoriAdı))
                    sorgu = sorgu.Where(c => c.Adı.Contains(kategoriAdı));
                sorgu = sorgu.Where(c => !c.Silindi);
                sorgu = sorgu.OrderBy(c => c.ParentKategoriId).ThenBy(c => c.GörüntülenmeSırası).ThenBy(c => c.Id);
                #region acl
                /*
                if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
                {
                    if (!showHidden && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }

                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }*/
                #endregion
                var sıralanmamışKategoriler = sorgu.ToList();
                var sıralananKategoriler = sıralanmamışKategoriler.KategorileriAğaçİçinSırala();
                return new SayfalıListe<Kategori>(sıralanmamışKategoriler, pageIndex, pageSize);
            }
        }
        public virtual IList<Kategori> TümKategorileriAlParentKategoriId(int parentKategoriId,
            bool GizliOlanlarıGöster=false,bool tümSeviyeleriDahilEt = false)
        {
            string key = string.Format(KATEGORILER_BY_PARENT_KATEGORI_ID_KEY, parentKategoriId, GizliOlanlarıGöster, _workContext.MevcutKullanıcı.Id, _siteContext.MevcutSite.Id, tümSeviyeleriDahilEt);
            return _önbellekYönetici.Al(key, () =>
            {
                var sorgu = _kategoriDepo.Tablo;
                if (!GizliOlanlarıGöster)
                    sorgu = sorgu.Where(c => c.Yayınlandı);
                sorgu = sorgu.Where(c => c.ParentKategoriId == parentKategoriId);
                sorgu = sorgu.Where(c => !c.Silindi);
                sorgu = sorgu.OrderBy(c => c.GörüntülenmeSırası).ThenBy(c => c.Id);

                var kategoriler = sorgu.ToList();
                if (tümSeviyeleriDahilEt)
                {
                    var childKategorileri = new List<Kategori>();
                    foreach (var kategori in kategoriler)
                    {
                        childKategorileri.AddRange(TümKategorileriAlParentKategoriId(kategori.Id, GizliOlanlarıGöster, tümSeviyeleriDahilEt));
                    }
                    kategoriler.AddRange(childKategorileri);
                }
                return kategoriler;
            });
        }
        private void KategoriGüncelle(Kategori kategori)
        {
            throw new NotImplementedException();
        }
    }
}
