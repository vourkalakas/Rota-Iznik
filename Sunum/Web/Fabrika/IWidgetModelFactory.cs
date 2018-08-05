using System.Collections.Generic;
using Web.Models.Cms;

namespace Web.Fabrika
{
    public partial interface IWidgetModelFactory
    {
        List<RenderWidgetModel> PrepareRenderWidgetModel(string widgetZone, object additionalData = null);
    }
}
