using Data.Entities;
using Data.Enums;
using Microsoft.Maui.Controls.Compatibility;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Data.Models
{
    public abstract class EntryModel
    {
        public ObjectId EntityId { get; }
        public List<TranslationModel> Translations { get; } = new List<TranslationModel>();
        public SourceModel Source { get; }
        public int Rate { get; }
        public EntryModel(Entities.Entry entry)
        {
            EntityId = entry._id;
            Source = new SourceModel(entry.Source);
            Rate = entry.Rate;
            foreach (var translationEntity in entry.Translations)
            {
                Translations.Add(new TranslationModel(translationEntity));
            }
        }
    }
}
