using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class NounModel : WordModel
    {

        // One of 6 grammatical classes    
        public int Class { get; set; }

        // Cклонение
        // 4 Declensions of nouns
        // https://nohchalla.com/literatura/chechenskiy-yazyk/audio-yazyk/791-urok28
        public NounDeclension Declension { get; set; }
        public NameType NameType { get; set; }
        public NumericalType NumericalType { get; set; }
    }
}
