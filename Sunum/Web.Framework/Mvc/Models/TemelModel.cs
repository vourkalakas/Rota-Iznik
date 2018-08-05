using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Framework.Mvc.Models
{
    public partial class TemelModel
    {
        #region Ctor
        public TemelModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }
        #endregion

        #region Methods
        public virtual void BindModel(ModelBindingContext bindingContext)
        {
        }
        protected virtual void PostInitialize()
        {
        }
        #endregion

        #region Properties
        public Dictionary<string, object> CustomProperties { get; set; }
        #endregion

    }
    public partial class TemelVarlıkModel : TemelModel
    {
        public virtual int Id { get; set; }
    }
}
