using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_utils.Services;
using MongoDB.Bson;

namespace chldr_data.Models
{

    public class WordModel : EntryModel
    {
        public WordModel(Entry entry) : this(entry.Word) { }
        public WordModel(Word word) : base(word.Entry)
        {
            Id = word._id;
            Content = word.Content;
            Notes = word.Notes;
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
        public new ObjectId Id { get; }
        public override string Content { get; }
        public string Notes { get; }
        public PartsOfSpeech PartOfSpeech { get; }
        public IWordDetails Characteristics { get; }
    }
}