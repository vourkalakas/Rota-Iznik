using System.Collections.Generic;
using Core.Domain.Localization;

namespace Services.Localization
{
    public partial interface ILanguageService
    {
        void DeleteLanguage(Dil language);
        IList<Dil> GetAllLanguages(bool showHidden = false, int storeId = 0, bool loadCacheableCopy = true);
        Dil GetLanguageById(int languageId, bool loadCacheableCopy = true);
        void InsertLanguage(Dil language);
        void UpdateLanguage(Dil language);
    }
}
