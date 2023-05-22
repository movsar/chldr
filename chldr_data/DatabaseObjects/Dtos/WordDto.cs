using chldr_data.Enums;
using chldr_data.Enums.WordDetails;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;

namespace chldr_data.DatabaseObjects.Dtos
{
    // ! All public properties of this class must have setters, to allow serialization
    public class WordDto : EntryDto, IWord
    {
        [JsonConstructor]
        public WordDto() { }

        public static WordDto FromModel(WordModel wordModel)
        {
            var wordDto = new WordDto()
            {
                EntryId = wordModel.EntryId,
                SourceId = wordModel.Source.SourceId,
                Rate = wordModel.Rate,
                EntryType = (EntryType)wordModel.Type,
                WordId = wordModel.WordId.ToString(),
                Content = wordModel.Content,
                Notes = wordModel.Notes,
                PartOfSpeech = wordModel.PartOfSpeech,
                AdditionalDetails = new WordDetailsModel(wordModel, wordModel.PartOfSpeech)
            };

            wordDto.Translations.AddRange(wordModel.Translations.Select(t => TranslationDto.FromModel(t)).ToList());
            return wordDto;
        }

        #region Main Details
        public string WordId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public PartOfSpeech PartOfSpeech { get; set; }
        #endregion
        public WordDetailsModel AdditionalDetails { get; set; }
    }
}
