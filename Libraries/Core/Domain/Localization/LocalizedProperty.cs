﻿namespace Core.Domain.Localization
{
    public partial class LocalizedProperty : TemelVarlık
    {
        public int EntityId { get; set; }
        public int LanguageId { get; set; }
        public string LocaleKeyGroup { get; set; }
        public string LocaleKey { get; set; }
        public string LocaleValue { get; set; }
        public virtual Dil Language { get; set; }
    }
}
