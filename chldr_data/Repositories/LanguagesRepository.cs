using chldr_data.Dto;
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
    public class LanguagesRepository : EntriesRepository<PhraseModel>
    {
        public LanguagesRepository(IRealmService realmService) : base(realmService) { }

        public ObjectId Insert(LanguageDto languageInfo)
        {
            ObjectId insertedEntryId = new ObjectId();
            try
            {
                Database.Write(() =>
                {
                    var languageEntity = new Language()
                    {
                        Code = languageInfo.Code,
                        Name = languageInfo.Name
                    };

                    Database.Add(languageEntity);
                    insertedEntryId = languageEntity._id;
                });

                return insertedEntryId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PhraseModel GetById(ObjectId entityId)
        {
            return new PhraseModel(Database.All<Phrase>().FirstOrDefault(p => p._id == entityId));
        }
    }
}
