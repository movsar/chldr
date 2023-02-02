using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Services;
using MongoDB.Bson;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace chldr_data.Models
{
    public abstract class EntryModel : ModelBase
    {
        public override ObjectId Id { get; }
        public List<TranslationModel> Translations { get; } = new List<TranslationModel>();
        public SourceModel Source { get; }
        public int Rate { get; }
        public int Type { get; }
        public EntryModel(Entry entry)
        {
            Id = entry._id;
            Source = new SourceModel(entry.Source);
            Rate = entry.Rate;
            Type = entry.Type;
            foreach (var translationEntity in entry.Translations)
            {
                Translations.Add(new TranslationModel(translationEntity));
            }
        }
    }
}
