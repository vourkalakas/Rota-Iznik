using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Core.Altyapı;
using Services.Olaylar;
using Web.Framework.Olaylar;
using Web.Framework.Mvc.Models;

namespace Web.Framework.Bileşenler
{
    public abstract class ViewBileşeni : ViewComponent
    {
        private void HazırlananModeliYayınla<TModel>(TModel model)
        {
            if (model is TemelModel)
            {
                var olayYayınlayıcı = EngineContext.Current.Resolve<IOlayYayınlayıcı>();
                olayYayınlayıcı.ModelHazırlandı(model as TemelModel);
            }

            if (model is IEnumerable<TemelModel> modelCollection)
            {
                var olayYayınlayıcı = EngineContext.Current.Resolve<IOlayYayınlayıcı>();
                olayYayınlayıcı.ModelHazırlandı(modelCollection);
            }
        }
        public new ViewViewComponentResult View<TModel>(string viewName, TModel model)
        {
            HazırlananModeliYayınla(model);
            return base.View<TModel>(viewName, model);
        }
        public new ViewViewComponentResult View<TModel>(TModel model)
        {
            HazırlananModeliYayınla(model);
            return base.View<TModel>(model);
        }
        public new ViewViewComponentResult View(string viewName)
        {
            return base.View(viewName);
        }
    }
}
