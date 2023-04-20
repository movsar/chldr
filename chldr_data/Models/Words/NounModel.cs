using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;
using chldr_data.Models.Words;

namespace chldr_data.Models.Entries
{
    public class NounModel : WordModel
    {
        public NounModel(RealmEntry entry) : base(entry)
        {
        }

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
