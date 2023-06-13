using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class WordDetails : IDetails
    {
        public WordDetails()
        {
            // Empty constructor
        }
        public WordDetails(IEntry word, WordType partOfSpeech)
        {
            switch (partOfSpeech)
            {
                case WordType.Undefined:
                    break;
                case WordType.Verb:
                    var verb = word as Verb;
                    if (verb == null)
                    {
                        return;
                    }
                    Tense = verb.Tense;
                    Conjugation = verb.Conjugation;
                    Mood = verb.Mood;
                    NumericalType = verb.NumericalType;
                    Transitiveness = verb.Transitiveness;
                    Classes[0] = verb.Class;
                    break;
                case WordType.Noun:
                    break;
                case WordType.Adverb:
                    break;
                case WordType.Adjective:
                    break;
                case WordType.Numeral:
                    break;
                case WordType.Pronoun:
                    break;
                case WordType.Conjunction:
                    break;
                case WordType.Particle:
                    break;
                case WordType.Interjection:
                    break;
                case WordType.Masdar:
                    break;
                case WordType.Gerund:
                    break;
                default:
                    break;
            }
        }

        // Class can be 1,2,3,4,5 or 6
        // Should be available only for some verbs, and nouns so don't allow to set it for all verbs
        // Person can have 1,2 or 3 classes
        public int[] Classes { get; set; } = new int[] { 0, 0, 0 };

        #region Pronoun Details
        // Person can be 1,2 or 3
        public int Person { get; set; }
        #endregion

        #region Verb Details
        public VerbTense Tense { get; set; }
        public VerbConjugation Conjugation { get; set; }
        public VerbMood Mood { get; set; } = new();
        public NumericalType NumericalType { get; set; }
        public Transitiveness Transitiveness { get; set; }
        #endregion

        #region Adjective Details
        // качественные и относительные
        public AdjectiveSemanticType SemanticType { get; set; }
        // зависимые и независимые
        public AdjectiveCharacteristic Characteristic { get; set; }
        public Case Case { get; set; }
        // Степень - только у качественных
        public Degree? Degree { get; set; }
        // Грамматический класс - только у качественных и то, некоторых 
        #endregion

        #region Numeral Details
        public Complexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        #endregion

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
}
