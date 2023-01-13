using Data.Entities;
using Data.Enums;
using Data.Factories;
using Microsoft.Maui.Controls.Compatibility;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Data.Models
{
    public class EntryModel
    {
        public ObjectId EntryId { get; }
        public TargetModel Target { get; }
        public List<TranslationModel> Translations { get; } = new List<TranslationModel>();
        public SourceModel Source { get; }
        public string Notes { get; }
        public int Type { get; }

        public int Rate { get; }
        public EntryModel(Entities.Entry entry)
        {
            EntryId = entry._id;
            Type = entry.Type;
            Source = new SourceModel(entry.Source);
            Target = TargetModelFactory.CreateTarget(entry);
            Rate = entry.Rate;
            foreach (var translationEntity in entry.Translations)
            {
                Translations.Add(new TranslationModel(translationEntity));
            }
        }
    }
}
