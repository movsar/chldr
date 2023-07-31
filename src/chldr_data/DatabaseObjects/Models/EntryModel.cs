using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Enums.WordDetails;
using chldr_data.Helpers;
using chldr_data.Interfaces;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Models
{
    public class EntryModel : IEntry
    {
        public string EntryId { get; internal set; }
        public string UserId { get; set; }
        public string? SourceId => Source.SourceId;
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public EntryType Type { get; set; }
        public int Subtype { get; set; }
        public string Content { get; set; }
        public IDetails? Details { get; set; }
        public SourceModel Source { get; set; }
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
        public List<SoundModel> Sounds { get; set; } = new List<SoundModel>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public List<EntryModel> SubEntries { get; set; } = new List<EntryModel>();

        public static EntryModel FromEntity(IEntryEntity entry, ISourceEntity source, IEnumerable<ITranslationEntity> translations, IEnumerable<ISoundEntity> sounds)
        {
            var entryModel = new EntryModel()
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
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
                entryModel.Sounds.Add(SoundModel.FromEntity(sound));
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