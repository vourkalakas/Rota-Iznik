using System.Collections.Generic;
using System.IO;
using Core;
using Core.Eklentiler;
using Services.Cms;
using Services.Yapılandırma;
using Services.Medya;

namespace Eklentiler.Widgets.NivoSlider
{
    public class NivoSliderPlugin : TemelEklenti, IWidgetEklenti
    {
        private readonly IResimServisi _resimServisi;
        private readonly IAyarlarServisi _ayarlarServisi;
        private readonly IWebYardımcısı _webYardımcısı;

        public NivoSliderPlugin(IResimServisi resimServisi,
            IAyarlarServisi ayarlarServisi, IWebYardımcısı webYardımcısı)
        {
            this._resimServisi = resimServisi;
            this._ayarlarServisi = ayarlarServisi;
            this._webYardımcısı = webYardımcısı;
        }
        public IList<string> WidgetBölgeleriniAl()
        {
            return new List<string> { "home_page_top" };
        }
        public override string SayfaYapılandırmaUrlsiniAl()
        {
            return _webYardımcısı.SiteKonumuAl() + "Admin/WidgetsNivoSlider/Configure";
        }
        public void PublicViewBileşeniAl(string widgetZone, out string viewComponentName)
        {
            viewComponentName = "WidgetsNivoSlider";
        }
        public override void Yükle()
        {
            //pictures
            var sampleImagesPath = GenelYardımcı.MapPath("~/Eklentiler/Widgets.NivoSlider/Content/nivoslider/sample-images/");


            //settings
            var settings = new NivoSliderSettings
            {
                Picture1Id = _resimServisi.ResimEkle(File.ReadAllBytes(sampleImagesPath + "banner1.jpg"), MimeTipleri.ImagePJpeg, "banner_1").Id,
                Text1 = "",
                Link1 = _webYardımcısı.SiteKonumuAl(false),
                Picture2Id = _resimServisi.ResimEkle(File.ReadAllBytes(sampleImagesPath + "banner2.jpg"), MimeTipleri.ImagePJpeg, "banner_2").Id,
                Text2 = "",
                Link2 = _webYardımcısı.SiteKonumuAl(false),
                //Picture3Id = _resimServisi.InsertPicture(File.ReadAllBytes(sampleImagesPath + "banner3.jpg"), MimeTypes.ImagePJpeg, "banner_3").Id,
                //Text3 = "",
                //Link3 = _webYardımcısı.GetStoreLocation(false),
            };
            _ayarlarServisi.AyarKaydet(settings);

            /*
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture1", "Picture 1");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture2", "Picture 2");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture3", "Picture 3");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture4", "Picture 4");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture5", "Picture 5");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture.Hint", "Upload picture.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Text", "Comment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Text.Hint", "Enter comment for picture. Leave empty if you don't want to display any text.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Link", "URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");
            */
            base.Yükle();
        }
        public override void Sil()
        {
            //settings
            _ayarlarServisi.AyarSil<NivoSliderSettings>();
            /*
            //locales
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture1");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture2");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture3");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture4");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture5");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Text");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Text.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Link");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Link.Hint");
            */

            base.Sil();
        }
    }
}