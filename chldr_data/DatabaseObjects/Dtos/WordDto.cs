using chldr_data.Enums;
using chldr_data.Enums.WordDetails;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.DatabaseObjects.Dtos
{
    // ! All public properties of this class must have setters, to allow serialization
    public class WordDto : EntryDto, IWord
    {
     
        #region Main Details
        public string WordId { get; set; } = Guid.NewGuid().ToString();
        public override string Content { get; set; } = string.Empty;
        public PartOfSpeech PartOfSpeech { get; set; }
        #endregion
        public WordDetails AdditionalDetails { get; set; } = new();
        public static WordDto FromModel(WordModel model)
        {
            var wordDto = new WordDto()
            {
                WordId = model.WordId,
                Content = model.Content,
                PartOfSpeech = model.PartOfSpeech,
                
                //AdditionalDetails = 
            };

            wordDto.SetEntryFields(model);
            return wordDto;
        }
    }
}
