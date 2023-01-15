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
    public class TextModel : TargetModel
    {
        public ObjectId EntryId { get; }
        public string Content { get; }
        public List<TranslationModel> Translations = new List<TranslationModel>();

        public TextModel(Text text)
        {
            this.TargetId = text._id;
            this.EntryId = text.Entry._id;
            this.Content = text.Content;
            foreach (var translationEntity in text.Entry.Translations)
            {
                this.Translations.Add(new TranslationModel(translationEntity));
            }
        }
    }
}
