using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Olaylar;

namespace Web.Framework.Olaylar
{
    public static class OlayYayınlayıcıUzantıları
    {
        public static void ModelHazırlandı<T>(this IOlayYayınlayıcı olayYayınlayıcı, T model)
        {
            olayYayınlayıcı.Yayınla(new ModelHazırlandı<T>(model));
        }
        public static void ModelAlındı<T>(this IOlayYayınlayıcı olayYayınlayıcı, T model, ModelStateDictionary modelState)
        {
            olayYayınlayıcı.Yayınla(new ModelAlındı<T>(model, modelState));
        }
    }
}
