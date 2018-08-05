using System;
using System.Linq.Expressions;
using Core;
using Core.Domain.Localization;

namespace Services.Localization
{
    public partial interface ILocalizedEntityService
    {
        void DeleteLocalizedProperty(LocalizedProperty localizedProperty);
        LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId);
        string GetLocalizedValue(int languageId, int entityId, string localeKeyGroup, string localeKey);
        void InsertLocalizedProperty(LocalizedProperty localizedProperty);
        void UpdateLocalizedProperty(LocalizedProperty localizedProperty);
        void SaveLocalizedValue<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            int languageId) where T : TemelVarlık, ILocalizedEntity;
        void SaveLocalizedValue<T, TPropType>(T entity,
           Expression<Func<T, TPropType>> keySelector,
           TPropType localeValue,
           int languageId) where T : TemelVarlık, ILocalizedEntity;
    }
}
