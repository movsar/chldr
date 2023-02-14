using chldr_data.Entities;
using chldr_data.Enums;
using chldr_utils.Services;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class WordModel : EntryModel
    {
        public new ObjectId Id { get; }
        public override string Content { get; }
        public string Notes { get; }
        public string RawForms { get; }
        public List<string> Forms { get; } = new();
        public Dictionary<string, string> NounDeclensions { get; }
        public Dictionary<string, string> VerbTenses { get; }
        public PartsOfSpeech PartOfSpeech { get; }
        public List<int> GrammaticalClasses { get; } = new List<int>();
        public WordModel(Entry entry) : this(entry.Word) { }
        public WordModel(Word word) : base(word.Entry)
        {
            Id = word._id;
            Content = word.Content;
            Notes = word.Notes;
            RawForms = word.Forms;
            Forms = word.Forms.Split(";").ToList();
            GrammaticalClasses.AddRange(word.GrammaticalClasses);
            try
            {
                VerbTenses = Word.ParseVerbTenses(word.VerbTenses);
                NounDeclensions = Word.ParseNounDeclensions(word.NounDeclensions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while parsing word stuff");
                Console.WriteLine(ex.Message);
            }

            PartOfSpeech = (PartsOfSpeech)word.PartOfSpeech;
        }
    }
}
