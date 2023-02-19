using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
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

            // TODO: Build appropriate WordDetails
            // GrammaticalClasses.AddRange(word.GrammaticalClasses);

            PartOfSpeech = (PartOfSpeech)word.PartOfSpeech;
        }
        public new ObjectId Id { get; }
        public override string Content { get; }
        public string Notes { get; }
        public PartOfSpeech PartOfSpeech { get; }
        public IWordDetails Characteristics { get; }
    }
}