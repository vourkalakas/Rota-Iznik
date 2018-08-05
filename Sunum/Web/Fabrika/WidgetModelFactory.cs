using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Core;
using Core.Önbellek;
using Services.Cms;
using Web.Framework.Temalar;
//using Web.al.Cache;
using Web.Models.Cms;
using Web.Altyapı.Önbellek;

namespace Web.Fabrika
{
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
        #region Fields

        private readonly IWidgetServisi _widgetService;
        private readonly ISiteContext _storeContext;
        private readonly ITemaContext _themeContext;
        private readonly IStatikÖnbellekYönetici _cacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IWidgetServisi widgetService,
            ISiteContext storeContext,
            ITemaContext themeContext,
            IStatikÖnbellekYönetici cacheManager,
            IWorkContext workContext)
        {
            this._widgetService = widgetService;
            this._storeContext = storeContext;
            this._themeContext = themeContext;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }

        #endregion

        #region Methods
        public virtual List<RenderWidgetModel> PrepareRenderWidgetModel(string widgetZone, object additionalData = null)
        {
            var cacheKey = string.Format(ModelÖnbellekOlayTüketici.WIDGET_MODEL_KEY,
                _workContext.MevcutKullanıcı.Id, _storeContext.MevcutSite.Id, widgetZone, _themeContext.MevcutTemaAdı);
            
            additionalData = new RouteValueDictionary()
            {
                { "widgetZone", widgetZone },
                { "additionalData", additionalData }
            };

            var cachedModel = _cacheManager.Al(cacheKey, () =>
            {
                //model
                var model = new List<RenderWidgetModel>();

                var widgets = _widgetService.BölgedenAktifWidgetleriYükle(widgetZone, _workContext.MevcutKullanıcı, _storeContext.MevcutSite.Id);
                foreach (var widget in widgets)
                {
                    widget.PublicViewBileşeniAl(widgetZone, out string viewComponentName);

                    var widgetModel = new RenderWidgetModel
                    {
                        WidgetViewComponentName = viewComponentName,
                        WidgetViewComponentArguments = additionalData
                    };

                    model.Add(widgetModel);
                }
                return model;
            });
            var clonedModel = new List<RenderWidgetModel>();

            foreach (var widgetModel in cachedModel)
            {
                var clonedWidgetModel = new RenderWidgetModel
                {
                    WidgetViewComponentName = widgetModel.WidgetViewComponentName
                };

                if (widgetModel.WidgetViewComponentArguments != null)
                    clonedWidgetModel.WidgetViewComponentArguments = new RouteValueDictionary(widgetModel.WidgetViewComponentArguments);

                if (additionalData != null)
                {
                    if (clonedWidgetModel.WidgetViewComponentArguments == null)
                        clonedWidgetModel.WidgetViewComponentArguments = new RouteValueDictionary();

                    clonedWidgetModel.WidgetViewComponentArguments = additionalData;
                }

                clonedModel.Add(clonedWidgetModel);
            }

            return clonedModel;
        }

        #endregion
    }
}
