using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Core;

namespace Web.Framework.Mvc.ModelBağlama
{
    public class MetaDataSağlayıcı : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            var additionalValues = context.Attributes.OfType<IModelÖzelliği>().ToList();
            foreach (var additionalValue in additionalValues)
            {
                if (context.DisplayMetadata.AdditionalValues.ContainsKey(additionalValue.Adı))
                    throw new Hata("Zaten bu isimde özellik mevcut '{0}' on this model", additionalValue.Adı);

                context.DisplayMetadata.AdditionalValues.Add(additionalValue.Adı, additionalValue);
            }
        }
    }
}
