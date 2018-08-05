using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Framework.Olaylar
{
    public class SayfaRenderOlayı
    {
        public SayfaRenderOlayı(IHtmlHelper helper)
        {
            this.Helper = helper;
        }
        public IHtmlHelper Helper { get; private set; }
    }
}
