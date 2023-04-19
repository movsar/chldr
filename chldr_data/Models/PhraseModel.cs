using chldr_data.Entities;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class PhraseModel : EntryModel
    {
        public string PhraseId { get; }
        public override string Content { get; }
        public string? Notes { get; }
        public PhraseModel(Entry entry) : base(entry)
        {
            var phrase = entry.Phrase;

            PhraseId = phrase._id;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }
    }
}
