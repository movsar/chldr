using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models.Words
{
    public class WordModel : EntryModel, IWord
    {
        public WordModel(RealmEntry entry) : base(entry)
        {
            var word = entry.Word;

            WordId = word.WordId;
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = (PartOfSpeech)word.PartOfSpeech;
        }

        public WordModel(SqlEntry entry) : base(entry)
        {
            var word = entry.Word;

            WordId = word.WordId;
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = (PartOfSpeech)word.PartOfSpeech;
        }

        public string WordId { get; }
        public override string Content { get; }
        public string Notes { get; }
        public PartOfSpeech PartOfSpeech { get; }

        public override DateTimeOffset CreatedAt { get; }

        public override DateTimeOffset UpdatedAt { get; }
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