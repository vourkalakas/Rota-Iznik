using Microsoft.AspNetCore.Mvc;
using Web.Fabrika;
using Web.Framework.Components;

namespace Web.Components
{
    public class AdminHeaderLinksViewComponent : TemelViewComponent
    {
        private readonly IGenelModelFactory _commonModelFactory;

        public AdminHeaderLinksViewComponent(IGenelModelFactory commonModelFactory)
        {
            this._commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareAdminHeaderLinksModel();
            return View(model);
        }
    }
}
