using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.Models.WordDetails
{
    public class NounDetails : IWordDetails
    {
        // 6 Grammatical classes
        public int Class { get; set; }

        // Склонение
        // 4 Declensions of nouns
        // https://nohchalla.com/literatura/chechenskiy-yazyk/audio-yazyk/791-urok28
        public int Declension { get; set; }

        public Cases SingularCases { get; set; }
        public Cases PluralCases { get; set; }

        public NameType NameType { get; set; }
        public NumericalType NumericalType { get; set; }
    }
}
