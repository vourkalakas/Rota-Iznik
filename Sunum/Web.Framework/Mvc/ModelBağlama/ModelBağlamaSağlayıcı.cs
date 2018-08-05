using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Framework.Mvc.Models;

namespace Web.Framework.Mvc.ModelBağlama
{
    public class ModelBağlamaSağlayıcı : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));


            var modelType = context.Metadata.ModelType;
            if (!typeof(TemelModel).IsAssignableFrom(modelType))
                return null;

            if (context.Metadata.IsComplexType && !context.Metadata.IsCollectionType)
            {
                var propertyBinders = context.Metadata.Properties
                    .ToDictionary(modelProperty => modelProperty, modelProperty => context.CreateBinder(modelProperty));

                return new ModelBağlayıcı(propertyBinders);
            }
            
            return null;
        }
    }
}
