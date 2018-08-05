using Core.Önbellek;
using Core.Domain.Yapılandırma;
using Core.Olaylar;
using Services.Olaylar;

namespace Eklentiler.Widgets.NivoSlider.Altyapı.Önbellek
{
    public partial class ModelÖnbellekOlayTüketici :
        IMüşteri<OlayEklendi<Ayarlar>>,
        IMüşteri<OlayGüncellendi<Ayarlar>>,
        IMüşteri<OlaySilindi<Ayarlar>>
    {
        public const string PICTURE_URL_MODEL_KEY = "Nop.plugins.widgets.nivoslider.pictureurl-{0}";
        public const string PICTURE_URL_PATTERN_KEY = "Nop.plugins.widgets.nivoslider";

        private readonly IStatikÖnbellekYönetici _cacheManager;

        public ModelÖnbellekOlayTüketici(IStatikÖnbellekYönetici cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        public void Olay(OlayEklendi<Ayarlar> eventMessage)
        {
            _cacheManager.KalıpİleSil(PICTURE_URL_PATTERN_KEY);
        }
        public void Olay(OlayGüncellendi<Ayarlar> eventMessage)
        {
            _cacheManager.KalıpİleSil(PICTURE_URL_PATTERN_KEY);
        }
        public void Olay(OlaySilindi<Ayarlar> eventMessage)
        {
            _cacheManager.KalıpİleSil(PICTURE_URL_PATTERN_KEY);
        }
    }
}
