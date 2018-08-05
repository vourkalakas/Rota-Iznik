using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Framework.Olaylar
{
    public class ModelAlındı<T>
    {
        #region Ctor
        public ModelAlındı(T model, ModelStateDictionary modelState)
        {
            this.Model = model;
            this.ModelState = modelState;
        }
        #endregion

        #region Properties
        public T Model { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }
        #endregion
    }
}
