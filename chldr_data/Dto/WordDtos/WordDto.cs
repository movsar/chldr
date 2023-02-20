using chldr_data.Enums.WordDetails;
using chldr_data.Models.Words;
using MongoDB.Bson;

namespace chldr_data.Dto.WordDtos
{
    public class WordDto : EntryDto
    {
        public WordDto(WordModel word) : base(word)
        {
            WordId = word.WordId.ToString();
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = word.PartOfSpeech;
        }
        public WordDto() { }
        public string? WordId { get; }
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public PartOfSpeech PartOfSpeech { get; set; }
    }
}
