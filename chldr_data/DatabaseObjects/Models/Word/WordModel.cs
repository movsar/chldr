using chldr_data.Enums.WordDetails;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.SqlEntities;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class WordModel : EntryModel, IWord
    {
        public string WordId { get; set; }
        public string Notes { get; set; }
        public PartOfSpeech PartOfSpeech { get; set; }
        public override string Content { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }
        protected WordModel() { }
        public static WordModel FromEntity(IEntryEntity entry, IWordEntity word, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var wordModel = new WordModel()
            {
                EntryId = entry.EntryId,
                Rate = entry.Rate,
                Type = entry.Type,
                Source = SourceModel.FromEntity(source),
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,

                WordId = word.WordId,
                Content = word.Content,
                Notes = word.Notes,
                PartOfSpeech = (PartOfSpeech)word.PartOfSpeech
            };

            foreach (var translationEntityToLanguage in translationEntityInfos)
            {
                wordModel.Translations.Add(TranslationModel.FromEntity(translationEntityToLanguage.Value, translationEntityToLanguage.Key));
            }

            return wordModel;
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
}