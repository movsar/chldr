using chldr_data.Entities;
using chldr_data.Enums;
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
    public abstract class EntryModel
    {
        public ObjectId EntityId { get; }
        public List<TranslationModel> Translations { get; } = new List<TranslationModel>();
        public SourceModel Source { get; }
        public int Rate { get; }
        public EntryModel(Entities.Entry entry)
        {
            if (entry.Word == null && entry.Text == null && entry.Phrase == null)
            {
                var r = RealmService.GetRealm();
                r.Write(() =>
                {
                    r.Remove(entry);
                });
                var t = r.Subscriptions.WaitForSynchronizationAsync();
                t.Start();
                t.Wait();
                throw new Exception("entry is dirty");
            }

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
