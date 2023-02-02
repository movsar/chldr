using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Repositories
{
    public class PhrasesRepository : EntriesRepository<PhraseModel>
    {
        public PhrasesRepository(IRealmService realmService) : base(realmService) { }
        public PhraseModel Add(string content, string notes)
        {
            throw new NotImplementedException();
        }

        public void UpdatePhrase(UserModel loggedInUser, string? phraseId, string? content, string? notes)
        {
            throw new NotImplementedException();
        }

        public PhraseModel GetById(ObjectId entityId)
        {
            return new PhraseModel(Database.All<Phrase>().FirstOrDefault(p => p._id == entityId));
        }
    }
}
