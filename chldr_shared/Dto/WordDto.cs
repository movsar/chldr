using chldr_data.Enums;
using chldr_data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Dto
{
    public class WordDto : EntryDto
    {
        public WordDto(WordModel word) : base(word)
        {
            WordId = word.Id;
            Content = word.Content;
            Notes = word.Notes;
            GrammaticalClass = word.GrammaticalClass;
            PartOfSpeech = word.PartOfSpeech;

            foreach (var item in word.VerbTenses)
            {
                VerbTenses.Add(item.Key, item.Value);
            }

            foreach (var item in word.NounDeclensions)
            {
                NounDeclensions.Add(item.Key, item.Value);
            }
        }
        public WordDto() { }
        public ObjectId WordId { get; }
        Dictionary<string, string> Forms { get; } = new Dictionary<string, string>();
        Dictionary<string, string> NounDeclensions { get; } = new Dictionary<string, string>();
        Dictionary<string, string> VerbTenses { get; } = new Dictionary<string, string>();
        public string Content { get; set; }
        public string Notes { get; set; }
        public PartsOfSpeech PartOfSpeech { get; set; }
        public int GrammaticalClass { get; set; }
    }
}
