using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
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
            return new PhraseModel(Database.All<Phrase>().FirstOrDefault(p => p.PhraseId == entityId).Entry);
        }
    }
}
