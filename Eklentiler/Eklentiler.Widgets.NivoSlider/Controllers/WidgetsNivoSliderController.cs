using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Core;
using Core.Önbellek;
using Eklentiler.Widgets.NivoSlider.Models;
using Services.Yapılandırma;
using Services.Localization;
using Services.Medya;
using Services.Güvenlik;
using Services.Siteler;
using Web.Framework;
using Web.Framework.Controllers;

namespace Eklentiler.Widgets.NivoSlider.Controllers
{
    [Area(AreaAdları.Admin)]
    public class WidgetsNivoSliderController : TemelEklentiController
    {
        private readonly IWorkContext _workContext;
        private readonly ISiteServisi _siteServisi;
        private readonly IResimServisi _resimServisi;
        private readonly IAyarlarServisi _ayarlarServisi;
        private readonly IİzinServisi _izinServisi;
        private readonly ILocalizationService _localizationService;

        public WidgetsNivoSliderController(IWorkContext workContext,
            ISiteServisi siteServisi,
            IResimServisi resimServisi,
            IAyarlarServisi ayarlarServisi,
            IİzinServisi izinServisi,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._siteServisi = siteServisi;
            this._resimServisi = resimServisi;
            this._ayarlarServisi = ayarlarServisi;
            this._izinServisi = izinServisi;
            this._localizationService = localizationService;
        }
        public IActionResult Configure()
        {
            if (!_izinServisi.YetkiVer(StandartİzinSağlayıcı.EklentileriYönet))
                return ErişimEngellendiGörünümü();
            
            var siteGörünümü = this.AktifSiteKapsamYapılandırmaAl(_siteServisi, _workContext);
            var nivoSliderSettings = _ayarlarServisi.AyarYükle<NivoSliderSettings>(siteGörünümü);
            var model = new ConfigurationModel
            {
                Picture1Id = nivoSliderSettings.Picture1Id,
                Text1 = nivoSliderSettings.Text1,
                Link1 = nivoSliderSettings.Link1,
                Picture2Id = nivoSliderSettings.Picture2Id,
                Text2 = nivoSliderSettings.Text2,
                Link2 = nivoSliderSettings.Link2,
                Picture3Id = nivoSliderSettings.Picture3Id,
                Text3 = nivoSliderSettings.Text3,
                Link3 = nivoSliderSettings.Link3,
                Picture4Id = nivoSliderSettings.Picture4Id,
                Text4 = nivoSliderSettings.Text4,
                Link4 = nivoSliderSettings.Link4,
                Picture5Id = nivoSliderSettings.Picture5Id,
                Text5 = nivoSliderSettings.Text5,
                Link5 = nivoSliderSettings.Link5,
                ActiveStoreScopeConfiguration = siteGörünümü
            };

            if (siteGörünümü > 0)
            {
                model.Picture1Id_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Picture1Id, siteGörünümü);
                model.Text1_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Text1, siteGörünümü);
                model.Link1_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Link1, siteGörünümü);
                model.Picture2Id_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Picture2Id, siteGörünümü);
                model.Text2_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Text2, siteGörünümü);
                model.Link2_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Link2, siteGörünümü);
                model.Picture3Id_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Picture3Id, siteGörünümü);
                model.Text3_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Text3, siteGörünümü);
                model.Link3_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Link3, siteGörünümü);
                model.Picture4Id_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Picture4Id, siteGörünümü);
                model.Text4_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Text4, siteGörünümü);
                model.Link4_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Link4, siteGörünümü);
                model.Picture5Id_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Picture5Id, siteGörünümü);
                model.Text5_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Text5, siteGörünümü);
                model.Link5_OverrideForStore = _ayarlarServisi.AyarlarMevcut(nivoSliderSettings, x => x.Link5, siteGörünümü);
            }

            return View("~/Eklentiler/Widgets.NivoSlider/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_izinServisi.YetkiVer(StandartİzinSağlayıcı.EklentileriYönet))
                return ErişimEngellendiGörünümü();

            var siteGörünümü = this.AktifSiteKapsamYapılandırmaAl(_siteServisi, _workContext);
            var nivoSliderSettings = _ayarlarServisi.AyarYükle<NivoSliderSettings>(siteGörünümü);

            var previousPictureIds = new[]
            {
                nivoSliderSettings.Picture1Id,
                nivoSliderSettings.Picture2Id,
                nivoSliderSettings.Picture3Id,
                nivoSliderSettings.Picture4Id,
                nivoSliderSettings.Picture5Id
            };

            nivoSliderSettings.Picture1Id = model.Picture1Id;
            nivoSliderSettings.Text1 = model.Text1;
            nivoSliderSettings.Link1 = model.Link1;
            nivoSliderSettings.Picture2Id = model.Picture2Id;
            nivoSliderSettings.Text2 = model.Text2;
            nivoSliderSettings.Link2 = model.Link2;
            nivoSliderSettings.Picture3Id = model.Picture3Id;
            nivoSliderSettings.Text3 = model.Text3;
            nivoSliderSettings.Link3 = model.Link3;
            nivoSliderSettings.Picture4Id = model.Picture4Id;
            nivoSliderSettings.Text4 = model.Text4;
            nivoSliderSettings.Link4 = model.Link4;
            nivoSliderSettings.Picture5Id = model.Picture5Id;
            nivoSliderSettings.Text5 = model.Text5;
            nivoSliderSettings.Link5 = model.Link5;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update 
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture1Id, model.Picture1Id_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text1, model.Text1_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link1, model.Link1_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture2Id, model.Picture2Id_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text2, model.Text2_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link2, model.Link2_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture3Id, model.Picture3Id_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text3, model.Text3_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link3, model.Link3_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture4Id, model.Picture4Id_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text4, model.Text4_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link4, model.Link4_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture5Id, model.Picture5Id_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text5, model.Text5_OverrideForStore, siteGörünümü, false);
            _ayarlarServisi.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link5, model.Link5_OverrideForStore, siteGörünümü, false);
            */
            //now clear settings cache
            _ayarlarServisi.ÖnbelleğiTemizle();

            //get current picture identifiers
            var currentPictureIds = new[]
            {
                nivoSliderSettings.Picture1Id,
                nivoSliderSettings.Picture2Id,
                nivoSliderSettings.Picture3Id,
                nivoSliderSettings.Picture4Id,
                nivoSliderSettings.Picture5Id
            };

            //delete an old picture (if deleted or updated)
            foreach (var pictureId in previousPictureIds.Except(currentPictureIds))
            {
                var previousPicture = _resimServisi.ResimAlId(pictureId);
                if (previousPicture != null)
                    _resimServisi.ResimSil(previousPicture);
            }

            BaşarılıBildirimi(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}
