namespace Core.Domain.Localization
{
    public partial class LocaleStringResource : TemelVarlık
    { 
        public int LanguageId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
        public virtual Dil Language { get; set; }
    }
}
