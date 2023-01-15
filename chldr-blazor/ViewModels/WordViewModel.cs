using Data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.ViewModels
{
    public class WordViewModel
    {
        public ObjectId EntityId { get; set; }
        public int PartOfSpeech { get; set; }
        public int GrammaticalClass { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }

        public WordViewModel(WordModel word)
        {
            EntityId = word.EntityId;
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = word.PartOfSpeech;
        }

        public WordViewModel()
        {
        }
    }
}
