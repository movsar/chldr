using chldr_data.Entities;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class PhraseModel : EntryModel
    {
        // This is Phrase Id, the Entry Id is in its parent
        public new ObjectId Id { get; }
        public override string Content { get; }
        public string? Notes { get; }
        public PhraseModel(Entry entry) : this(entry.Phrase) { }
        public PhraseModel(Phrase phrase) : base(phrase.Entry)
        {
            Id = phrase._id;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }
    }
}
