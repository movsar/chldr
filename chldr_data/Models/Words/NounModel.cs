using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.Models.Entries
{
    public class NounModel : WordModel
    {
        public NounModel(Entry entry) : base(entry)
        {
        }

        // 6 Grammatical classes    
        public int Class { get; set; }

        // Падежи по склонениям
        // 4 Declensions of nouns
        // https://nohchalla.com/literatura/chechenskiy-yazyk/audio-yazyk/791-urok28
        public Dictionary<NounDeclension, Case> Cases { get; set; }
        public NameType NameType { get; set; }
        public NumericalType NumericalType { get; set; }
    }
}
