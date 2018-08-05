using System.ComponentModel;
using Core;
using Core.Altyapı;
using Services.Localization;

namespace Web.Framework.Mvc.ModelBağlama
{
    public class RotaDisplayNameAttribute : DisplayNameAttribute, IModelÖzelliği
    {
        #region Fields

        private string _resourceValue = string.Empty;

        #endregion

        #region Ctor
        public RotaDisplayNameAttribute(string resourceKey) : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        #endregion

        #region Properties
        public string ResourceKey { get; set; }
        public override string DisplayName
        {
            get
            {
                var workingLanguageId = EngineContext.Current.Resolve<IWorkContext>().MevcutDil.Id;
                _resourceValue = EngineContext.Current.Resolve<ILocalizationService>().GetResource(ResourceKey, workingLanguageId, true, ResourceKey);
                return _resourceValue;
            }
        }
        public string Adı
        {
            get { return nameof(RotaDisplayNameAttribute); }
        }

        #endregion
    }
}
