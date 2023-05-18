using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces.DatabaseEntities;
using chldr_data.Models.Words;
using Newtonsoft.Json;

namespace chldr_data.Dto
{
    // ! All public properties of this class must have setters, to allow serialization
    public class WordDto : EntryDto, IWordDto
    {
        [JsonConstructor]
        public WordDto() { }

        public WordDto(List<TranslationDto> translations)
        {
            Translations = translations;
        }
        public WordDto(SqlWord sqlWord) : this(new WordModel(sqlWord.Entry)) { }
        public WordDto(WordModel word) : base(word)
        {
            WordId = word.WordId.ToString();
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = word.PartOfSpeech;
            AdditionalDetails = new WordDetailsModel(word, PartOfSpeech);
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
