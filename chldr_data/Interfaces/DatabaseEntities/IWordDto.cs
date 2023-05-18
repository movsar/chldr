using chldr_data.Enums.WordDetails;
using chldr_data.Models.Words;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IWordDto : IWord
    {
        public PartOfSpeech PartOfSpeech { get; set; }
        WordDetailsModel AdditionalDetails { get; set; }
    }
}
