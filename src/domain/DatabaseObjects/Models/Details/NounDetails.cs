using domain.Interfaces;
using domain.Enums.WordDetails;

namespace domain.DatabaseObjects.Models.Words
{
    public class NounDetails : IDetails
    {
        // One of 6 grammatical classes    
        public int Class { get; set; }
        // Cклонение
        // 4 Declensions of nouns
        // https://nohchalla.com/literatura/chechenskiy-yazyk/audio-yazyk/791-urok28
        public NounDeclension Declension { get; set; }
        public Case Case { get; set; }
        public NameType NameType { get; set; }
        public NumeralType NumeralType { get; set; }
    }
}
