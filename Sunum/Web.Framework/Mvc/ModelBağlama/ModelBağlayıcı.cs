using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Web.Framework.Mvc.Models;

namespace Web.Framework.Mvc.ModelBağlama
{
    public class ModelBağlayıcı : ComplexTypeModelBinder
    {
        #region Ctor
        public ModelBağlayıcı(IDictionary<ModelMetadata, IModelBinder> propertyBinders) : base(propertyBinders)
        {
        }

        #endregion

        #region Methods
        protected override object CreateModel(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var model = base.CreateModel(bindingContext);
            if (model is TemelModel)
                (model as TemelModel).BindModel(bindingContext);

            return model;
        }
        protected override void SetProperty(ModelBindingContext bindingContext, string modelName,
            ModelMetadata propertyMetadata, ModelBindingResult bindingResult)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueAsString = bindingResult.Model as string;
            if (bindingContext.Model is TemelModel && !string.IsNullOrEmpty(valueAsString))
            {
                var noTrim = (propertyMetadata as DefaultModelMetadata)?.Attributes?.Attributes?.OfType<TrimÖzelliğiYok>().Any();
                if (!noTrim.HasValue || !noTrim.Value)
                    bindingResult = ModelBindingResult.Success(valueAsString.Trim());
            }

            base.SetProperty(bindingContext, modelName, propertyMetadata, bindingResult);
        }

        #endregion
    }
}
