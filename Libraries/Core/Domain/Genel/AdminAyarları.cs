using Core.Yapılandırma;

namespace Core.Domain.Genel
{
    public partial class AdminAyarları:IAyarlar
    {
        public int VarsayılanGridSayfaBüyüklüğü { get; set; }
        public int VarsayılanPopupSayfaBüyüklüğü { get; set; }
        public string GridSayfaBüyüklükleri { get; set; }
        public string RichEditorEkAyarları { get; set; }
        public bool RichEditorJavaScriptİzinli { get; set; }
        public bool MesajTemalarındaRichEditorKullan { get; set; }
        public bool AdminBölgesindeReklamlarıGizle { get; set; }
        public string SonHaberBaşlıkları { get; set; }
        public bool JsondaIsoTarihdönüşümüKullan { get; set; }

    }
}
