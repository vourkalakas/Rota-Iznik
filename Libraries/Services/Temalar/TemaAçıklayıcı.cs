using Core.Eklentiler;
using Newtonsoft.Json;
namespace Services.Temalar
{
    public class TemaAçıklayıcı : IDescriptor
    {
        [JsonProperty(PropertyName = "SistemAdı")]
        public string SistemAdı { get; set; }
        [JsonProperty(PropertyName = "KısaAd")]
        public string KısaAd { get; set; }
        [JsonProperty(PropertyName = "ResimUrlsiniGörüntüle")]
        public string ResimUrlsiniGörüntüle { get; set; }
        [JsonProperty(PropertyName = "YazıyıGörüntüle")]
        public string YazıyıGörüntüle { get; set; }
    }
}
