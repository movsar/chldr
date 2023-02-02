using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_utils;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Realms.Sync.MongoClient;

namespace chldr_data.Repositories
{
    public class WordsRepository : EntriesRepository<WordModel>
    {
        public WordsRepository(IRealmService realmService) : base(realmService) { }


        public WordModel GetById(ObjectId entityId)
        {
            return new WordModel(Database.All<Word>().FirstOrDefault(w => w._id == entityId));
        }
    }
}
