using System;
using System.Linq;
using Core;
using Core.Domain;
using Core.Domain.Kullanıcılar;
using Services.Genel;
using Services.Temalar;

namespace Web.Framework.Temalar
{
    public partial class TemaContext : ITemaContext
    {
        #region Fields

        private readonly IGenelÖznitelikServisi _genelÖznitelikServisi;
        private readonly ISiteContext _siteContext;
        private readonly ITemaSağlayıcı _temaSağlayıcı;
        private readonly IWorkContext _workContext;
        private readonly SiteBilgiAyarları _siteBilgiAyarları;

        private string _önbelleklenmişTemaAdı;

        #endregion

        #region Ctor
        public TemaContext(IGenelÖznitelikServisi genericAttributeService,
            ISiteContext storeContext,
            ITemaSağlayıcı temaSağlayıcı,
            IWorkContext workContext,
            SiteBilgiAyarları storeInformationSettings)
        {
            this._genelÖznitelikServisi = genericAttributeService;
            this._siteContext = storeContext;
            this._temaSağlayıcı = temaSağlayıcı;
            this._workContext = workContext;
            this._siteBilgiAyarları = storeInformationSettings;
        }

        #endregion

        #region Properties
        public string MevcutTemaAdı
        {
            get
            {
                if (!string.IsNullOrEmpty(_önbelleklenmişTemaAdı))
                    return _önbelleklenmişTemaAdı;

                var temaAdı = string.Empty;

                if (_siteBilgiAyarları.KullanıcılarTemaSeçebilsin && _workContext.MevcutKullanıcı != null)
                    temaAdı = _workContext.MevcutKullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.MevcutTemaAdı, _genelÖznitelikServisi, _siteContext.MevcutSite.Id);

                if (string.IsNullOrEmpty(temaAdı))
                    temaAdı = _siteBilgiAyarları.MevcutSiteTeması;

                if (!_temaSağlayıcı.TemaMevcut(temaAdı))
                {
                    temaAdı = _temaSağlayıcı.TemalarıAl().FirstOrDefault()?.SistemAdı
                        ?? throw new Exception("Tema yüklenemedi");
                }
                
                this._önbelleklenmişTemaAdı = temaAdı;

                return temaAdı;
            }
            set
            {
                if (!_siteBilgiAyarları.KullanıcılarTemaSeçebilsin || _workContext.MevcutKullanıcı == null)
                    return;

                _genelÖznitelikServisi.ÖznitelikKaydet(_workContext.MevcutKullanıcı,
                    SistemKullanıcıÖznitelikAdları.MevcutTemaAdı, value, _siteContext.MevcutSite.Id);

                this._önbelleklenmişTemaAdı = null;
            }
        }

        #endregion
    }
}
