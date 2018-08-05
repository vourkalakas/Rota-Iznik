
using Microsoft.AspNetCore.Mvc;
using Web.Fabrika;
using Web.Framework.Components;

namespace Web.Components
{
    public class HeaderLinksViewComponent : TemelViewComponent
    {
        private readonly IGenelModelFactory _commonModelFactory;

        public HeaderLinksViewComponent(IGenelModelFactory commonModelFactory)
        {
            this._commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareHeaderLinksModel();
            return View(model);
        }
    }
}
