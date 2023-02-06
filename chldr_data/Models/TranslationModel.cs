using chldr_data.Entities;
using chldr_data.Factories;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class TranslationModel : ModelBase
    {
        public ObjectId? EntryId { get; }
        public string Content { get; }
        public string Notes { get; }
        public LanguageModel Language { get; }
        public int Rate { get; set; }
        public TranslationModel(Translation translation) : base(translation)
        {
            EntryId = translation.Entry._id;
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
        }
    }
}
