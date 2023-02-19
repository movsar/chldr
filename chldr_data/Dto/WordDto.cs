using chldr_data.Enums.WordDetails;
using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Dto
{
    public class WordDto : EntryDto
    {
        public WordDto(WordModel word) : base(word)
        {
            WordId = word.Id.ToString();
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = word.PartOfSpeech;
        }
        public WordDto() { }
        public string? WordId { get; }
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public PartOfSpeech PartOfSpeech { get; set; }
        public List<int> GrammaticalClasses { get; } = new List<int>();
    }
}
