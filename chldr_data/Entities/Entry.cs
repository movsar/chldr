using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;

namespace chldr_data.Entities
{
    public class Entry : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public User User { get; set; }
        public Word Word { get; set; }
        public Phrase Phrase { get; set; }
        public Text Text { get; set; }
        public Source Source { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        // Used to increase search speed
        public string RawContents { get; set; }
        public IList<Translation> Translations { get; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
