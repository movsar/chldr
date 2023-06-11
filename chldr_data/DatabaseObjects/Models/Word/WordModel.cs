using chldr_data.Enums.WordDetails;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class WordModel : EntryModel, IWord
    {
        protected WordModel() { }
        public string WordId { get; set; }
        public PartOfSpeech PartOfSpeech { get; set; }
        public override string Content { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }
        public static WordModel FromEntity(IWordEntity word, IEntryEntity entry, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var wordModel = new WordModel()
            {
                WordId = word.WordId,
                Content = word.Content,
                PartOfSpeech = (PartOfSpeech)word.PartOfSpeech
            };

            wordModel.SetEntryFields(entry, source, translationEntityInfos);
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