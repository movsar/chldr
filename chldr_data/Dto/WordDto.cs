using chldr_data.Enums;
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
            GrammaticalClasses.AddRange(word.GrammaticalClasses);
            PartOfSpeech = word.PartOfSpeech;

            foreach (var item in word.VerbTenses)
            {
                VerbTenses.Add(item.Key, item.Value);
            }

            foreach (var item in word.NounDeclensions)
            {
                NounDeclensions.Add(item.Key, item.Value);
            }
        }
        public WordDto() { }
        public string? WordId { get; }
        public List<string> Forms { get; } = new List<string>();
        public Dictionary<string, string> NounDeclensions { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> VerbTenses { get; } = new Dictionary<string, string>();
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public PartsOfSpeech PartOfSpeech { get; set; }
        public List<int> GrammaticalClasses { get; } = new List<int>();
    }
}
