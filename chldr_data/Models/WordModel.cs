using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;
using chldr_utils.Services;
using MongoDB.Bson;

namespace chldr_data.Models
{

    public class WordModel : EntryModel
    {
        public WordModel(Entry entry) : base(entry)
        {
            var word = entry.Word;

            WordId = word._id;
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = (PartOfSpeech)word.PartOfSpeech;
        }

        public ObjectId WordId { get; }
        public override string Content { get; }
        public string Notes { get; }
        public PartOfSpeech PartOfSpeech { get; }
        public IWordDetails Characteristics { get; }
    }
}