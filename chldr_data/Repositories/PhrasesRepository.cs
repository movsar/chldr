using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class PhrasesRepository : EntriesRepository<PhraseModel>
    {
        public PhrasesRepository(IDataAccess dataAccess) : base(dataAccess) { }
        public PhraseModel Add(string content, string notes)
        {
            throw new NotImplementedException();
        }

        public void UpdatePhrase(UserModel loggedInUser, string? phraseId, string? content, string? notes)
        {
            throw new NotImplementedException();
        }

        public PhraseModel GetById(string entityId)
        {
            return  PhraseModel.FromEntity(Database.All<RealmPhrase>().FirstOrDefault(p => p.PhraseId == entityId));
        }
    }
}
