using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models.Words;
using core.Enums;
using core.Enums.WordDetails;
using core.Helpers;

namespace core.DatabaseObjects.Models
{
    public class EntryModel : IEntry
    {
        public string EntryId { get; set; }
        public string UserId { get; set; }
        public string SourceId => Source.SourceId;
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public EntryType Type { get; set; }
        public int Subtype { get; set; }
        public string Content { get; set; }
        public object? Details { get; set; }
        public SourceModel Source { get; set; }
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
        public List<PronunciationModel> Sounds { get; set; } = new List<PronunciationModel>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public List<EntryModel> SubEntries { get; set; } = new List<EntryModel>();

        protected static string? ParseSource(string sourceName)
        {
            string? sourceTitle = null;
            switch (sourceName)
            {
                case "Maciev":
                    sourceTitle = "Чеченско - русский словарь, А.Г.Мациева";
                    break;
                case "Karasaev":
                    sourceTitle = "Русско - чеченский словарь, Карасаев А.Т., Мациев А.Г.";
                    break;
                case "User":
                    sourceTitle = "Добавлено пользователем";
                    break;
                case "Malaev":
                    sourceTitle = "Чеченско - русский словарь, Д.Б. Малаева";
                    break;
                case "Anatslovar":
                    sourceTitle = "Чеченско-русский, русско-чеченский словарь анатомии человека, Р.У. Берсанова";
                    break;
                case "ikhasakhanov":
                    sourceTitle = "Ислам Хасаханов";
                    break;
            }
            return sourceTitle;
        }
        private string GetHeader()
        {
            string header = Content!;
            string className = string.Empty;

            if (Details != null)
            {
                switch ((WordType)Subtype)
                {

                    case WordType.Noun:
                        var details = Details as NounDetails;
                        if (details?.Class != 0)
                        {
                            className = GrammaticalClassToString(details!.Class);
                        }
                        break;

                    case WordType.Verb:
                        break;

                    default:
                        Console.WriteLine("no handler for the details of this type");
                        break;
                }
                if (!string.IsNullOrEmpty(className))
                {
                    header = string.Join(" ", header, className);
                }
            }
            return header;
        }
        public string? Header => GetHeader();

        public string? Subheader => ParseSource(Source.Name);
        public static string GrammaticalClassToString(int grammaticalClass)
        {
            var ClassesMap = new Dictionary<int, string>()
            {
                { 1 ,"в, б/д"},
                { 2 ,"й, б/д"},
                { 3 ,"й, й"},
                { 4 ,"д, д"},
                { 5 ,"б, б/й"},
                { 6 ,"б, д"},
            };

            if (ClassesMap.TryGetValue(grammaticalClass, out string? classString))
            {
                return classString;
            }
            else
            {
                return "";
            }
        }

        public static EntryModel FromEntity(IEntryEntity entry, ISourceEntity source, IEnumerable<ITranslationEntity> translations, IEnumerable<ISoundEntity> sounds)
        {
            var entryModel = new EntryModel()
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId!,
                ParentEntryId = entry.ParentEntryId,
                Rate = entry.Rate,
                Type = (EntryType)entry.Type,
                Subtype = entry.Subtype,
                Source = SourceModel.FromEntity(source),
                Content = entry.Content,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };

            if (!string.IsNullOrEmpty(entry.Details))
            {
                switch (entryModel.Type)
                {
                    case EntryType.Word:
                        entryModel.Details = WordHelper.DeserializeWordDetails((WordType)entry.Subtype, entry.Details);
                        break;
                }
            }

            foreach (var translation in translations)
            {
                entryModel.Translations.Add(TranslationModel.FromEntity(translation));
            }

            foreach (var sound in sounds)
            {
                entryModel.Sounds.Add(PronunciationModel.FromEntity(sound));
            }

            return entryModel;
        }

        public static EntryModel FromEntity(IEntryEntity entry, ISourceEntity source, IEnumerable<ITranslationEntity> translations, IEnumerable<ISoundEntity> sounds,
          IEnumerable<IEntryEntity> subEntries, Dictionary<string, ISourceEntity> subSources, Dictionary<string, IEnumerable<ITranslationEntity>> subTranslations, Dictionary<string, IEnumerable<ISoundEntity>> subSounds)
        {
            var entryModel = FromEntity(entry, source, translations, sounds);


            foreach (var subEntry in subEntries)
            {
                var subEntryModel = FromEntity(subEntry, subSources[subEntry.EntryId], subTranslations[subEntry.EntryId], subSounds[subEntry.EntryId]);

                entryModel.SubEntries.Add(subEntryModel);
            }

            return entryModel;
        }
    }
}
/*

     Noun
       Declensions []
       Grammatical class
       Name type
         Proper name
         Common name
       Numerical
         Singular
         Plural

     Verb
       Class mutations
         Yes
         No
       Transitiveness
       Tense
         Conjugation
       Mood

     Numeral
       Numerical type
       Complexity
       Declension

     Adverb

     Pronoun
       Grammatical Class []
       Person
         1
         2
         3

     Conjunction
     Particle
     Interjection
     Masdar
     Gerund

     Существительное
       Склонение
       Грамматический класс
       Имя
         Собственное
         Нарицательное
       Число
         Единственное
         Множественное

     Глагол
       Изменяемость класса
         Да
         Нет
       Переходность
       Время
         Спряжение
       Наклонение

     Числительное
       Тип
       Сложность
       Падеж

     Наречие

     Местоимение
       Грамматический класс []
       Лицо	
         1
         2
         3

     Союз
     Частица
     Междометье
     Масдар
     Деепричастие

      */