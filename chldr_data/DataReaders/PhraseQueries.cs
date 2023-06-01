using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.RealmEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Readers
{
    public class PhraseQueries : DataQueries<RealmPhrase, PhraseModel>
    {
        public PhraseModel GetByEntryId(string entryId)
        {
            var word = Database.Find<RealmEntry>(entryId)!.Phrase;
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return PhraseModel.FromEntity(word);
        }

        public PhraseModel GetById(string phraseId)
        {
            var word = Database.All<RealmPhrase>().FirstOrDefault(w => w.PhraseId == phraseId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return PhraseModel.FromEntity(word);
        }
    }
}
