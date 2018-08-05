using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Core.Altyapı;
using Services.Olaylar;
using Web.Framework.Olaylar;
using Web.Framework.Mvc.Models;

namespace Web.Framework.Components
{
    public abstract class TemelViewComponent : ViewComponent
    {
        private void PublishModelPrepared<TModel>(TModel model)
        {
            if (model is TemelModel)
            {
                var eventPublisher = EngineContext.Current.Resolve<IOlayYayınlayıcı>();
                eventPublisher.ModelHazırlandı(model as TemelModel);
            }

            if (model is IEnumerable<TemelModel> modelCollection)
            {
                var eventPublisher = EngineContext.Current.Resolve<IOlayYayınlayıcı>();
                eventPublisher.ModelHazırlandı(modelCollection);
            }
        }
        public new ViewViewComponentResult View<TModel>(string viewName, TModel model)
        {
            PublishModelPrepared(model);

            return base.View<TModel>(viewName, model);
        }
        public new ViewViewComponentResult View<TModel>(TModel model)
        {
            PublishModelPrepared(model);

            return base.View<TModel>(model);
        }
        public new ViewViewComponentResult View(string viewName)
        {
            return base.View(viewName);
        }
    }
}
