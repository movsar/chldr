using chldr_data.Enums.WordDetails;

namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IWordDetails
    {
        #region Pronoun Details
        int Person { get; set; }
        #endregion

        #region Verb Details
        VerbMood Mood { get; set; }
        VerbConjugation Conjugation { get; set; }
        VerbTense Tense { get; set; }
        Transitiveness Transitiveness { get; set; }
        NumericalType NumericalType { get; set; }
        #endregion

        #region Numeral Details
        Complexity Complexity { get; set; }
        NumericalCategory Category { get; set; }
        #endregion

        #region Adjective Details
        int[] Classes { get; set; }
        Case Case { get; set; }
        AdjectiveCharacteristic Characteristic { get; set; }
        Degree? Degree { get; set; }
        AdjectiveSemanticType SemanticType { get; set; }
        #endregion
    }
}