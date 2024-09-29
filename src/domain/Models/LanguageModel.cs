namespace domain.Models
{
    public class LanguageModel : ILanguage
    {
        private LanguageModel() { }
        public string Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public LanguageModel(string code, string name)
        {
            Code = code;
            Name = name;
        }
        public static List<LanguageModel> GetAvailableLanguages()
        {
            return new List<LanguageModel>
            {
                new LanguageModel("RUS", "Русский"),
                new LanguageModel("ENG", "English"),
                new LanguageModel("CHE", "Нохчийн"),
                new LanguageModel("KAT", "ქართული ენა"),
                new LanguageModel("KOR", "한국어"),
                new LanguageModel("FRA", "Français"),
                new LanguageModel("DEU", "Deutsch"),
                new LanguageModel("NLD", "Nederlands"),
                new LanguageModel("ESP", "Español"),
                new LanguageModel("ARA", "اَلْعَرَبِيَّةُ"),
                new LanguageModel("POR", "Português"),
                new LanguageModel("INH", "ГӀалгӀайн"),
                new LanguageModel("TAT", "Татарча"),
                new LanguageModel("LAT", "Latin")
            };
        }
    }
}
