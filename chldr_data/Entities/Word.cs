using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System.Text;

namespace chldr_data.Entities
{
    public class Word : RealmObject, IEntity
    {
        // Must be removed with migrations ===============<
        internal int GrammaticalClass { get; set; }
        internal string Forms { get; set; } = string.Empty;
        internal string VerbTenses { get; set; } = string.Empty;
        internal string NounDeclensions { get; set; } = string.Empty;
        // ================================================

        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public Entry Entry { get; set; }
        [Indexed]
        public string Content { get; set; } = string.Empty;
        [Indexed]
        public string Notes { get; set; } = string.Empty;
        public IList<int> GrammaticalClasses { get; } = new List<int>();
        public int PartOfSpeech { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}