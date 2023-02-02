using chldr_data.Entities;
using chldr_data.Enums;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class WordModel : EntryModel
    {
        public WordModel(Entities.Word word) : base(word.Entry)
        {
            WordId = word._id;
            Content = word.Content;
            Notes = word.Notes;
            RawForms = word.Forms;
            GrammaticalClass = word.GrammaticalClass;
            VerbTenses = Word.ParseVerbTenses(word.VerbTenses);
            NounDeclensions = Word.ParseNounDeclensions(word.NounDeclensions);
            PartOfSpeech = (PartsOfSpeech)word.PartOfSpeech;
        }

        public WordModel(Entry entry) : this(entry.Word) { }
        public ObjectId WordId { get; }
        public string Content { get; }
        public string Notes { get; }
        public string RawForms { get; }
        public Dictionary<string, string> Forms { get; set; }
        public Dictionary<string, string> NounDeclensions { get; set; }
        public Dictionary<string, string> VerbTenses { get; set; }
        public PartsOfSpeech PartOfSpeech { get; }
        public int GrammaticalClass { get; internal set; }
    }
}
