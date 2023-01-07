using Data.Entities;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class TranslationModel
    {
        public ObjectId TranslationId { get; }
        public ObjectId EntryId { get; }
        public string Content { get; }
        public string Notes { get; }
        public LanguageModel Language { get; }
        public int Rate { get; set; }
        public TranslationModel(TranslationEntity translation)
        {
            TranslationId = translation._id;
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
        }
    }
}
