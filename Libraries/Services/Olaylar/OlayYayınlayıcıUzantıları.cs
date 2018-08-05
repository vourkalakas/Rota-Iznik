using Core;
using Core.Olaylar;

namespace Services.Olaylar
{
    public static class EventPublisherExtensions
    {
        public static void OlayEklendi<T>(this IOlayYayınlayıcı olayYayınlayıcı, T varlık) where T : TemelVarlık
        {
            olayYayınlayıcı.Yayınla(new OlayEklendi<T>(varlık));
        }

        public static void OlayGüncellendi<T>(this IOlayYayınlayıcı olayYayınlayıcı, T varlık) where T : TemelVarlık
        {
            olayYayınlayıcı.Yayınla(new OlayGüncellendi<T>(varlık));
        }

        public static void OlaySilindi<T>(this IOlayYayınlayıcı olayYayınlayıcı, T varlık) where T : TemelVarlık
        {
            olayYayınlayıcı.Yayınla(new OlaySilindi<T>(varlık));
        }
    }
}
