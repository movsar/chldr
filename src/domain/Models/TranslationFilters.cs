namespace domain.Models
{
    public class TranslationFilters
    {
        public bool? IncludeOnModeration { get; set; }
        public string[]? LanguageCodes { get; set; }
    }
}
