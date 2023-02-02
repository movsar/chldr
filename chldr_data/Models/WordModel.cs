using chldr_data.Entities;
using chldr_data.Enums;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class WordModel : EntryModel
    {
        public new ObjectId Id { get; }
        public string Content { get; }
        public string Notes { get; }
        public string RawForms { get; }
        public List<string> Forms { get; } = new();
        public Dictionary<string, string> NounDeclensions { get; }
        public Dictionary<string, string> VerbTenses { get; }
        public PartsOfSpeech PartOfSpeech { get; }
        public int GrammaticalClass { get; }
        public WordModel(Entry entry) : this(entry.Word) { }
        public WordModel(Word word) : base(word.Entry)
        {
            Id = word._id;
            Content = word.Content;
            Notes = word.Notes;
            RawForms = word.Forms;
            Forms = word.Forms.Split(";").ToList();
            GrammaticalClass = word.GrammaticalClass;
            VerbTenses = Word.ParseVerbTenses(word.VerbTenses);
            NounDeclensions = Word.ParseNounDeclensions(word.NounDeclensions);
            PartOfSpeech = (PartsOfSpeech)word.PartOfSpeech;
        }
    }
}
