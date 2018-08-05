using Web.Framework.Mvc.Models;

namespace Web.Models.Cms
{
    public partial class RenderWidgetModel : TemelModel
    {
        public string WidgetViewComponentName { get; set; }
        public object WidgetViewComponentArguments { get; set; }
    }
}
