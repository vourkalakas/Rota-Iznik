using Core.Yapılandırma;

namespace Core.Domain.Genel
{
    public partial class PdfAyarları:IAyarlar
    {
        public bool HarfSayfaBüyüklüğüEtkin { get; set; }
        public int LogoResimId { get; set; }
        public string FontDosyaAdı { get; set; }
    }
}
