using chldr_data.Entities;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class PhraseModel : EntryModel
    {
        public string PhraseId { get; }
        public override string Content { get; }
        public string? Notes { get; }

        public override DateTimeOffset CreatedAt { get; }

        public override DateTimeOffset UpdatedAt { get; }

        public PhraseModel(RealmEntry entry) : base(entry)
        {
            var phrase = entry.Phrase;

            PhraseId = phrase.PhraseId;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }
    }
}
