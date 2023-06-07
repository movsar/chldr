using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.Services;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

namespace chldr_data.Repositories
{
    public class RealmWordsRepository : RealmRepository<RealmWord, WordModel, WordDto>, IWordsRepository
    {
        public RealmWordsRepository(Realm context) : base(context)
        { }

        protected override RecordType RecordType => RecordType.Word;

        public override IEnumerable<ChangeSetModel> Add(string userId, WordDto dto)
        {
            throw new NotImplementedException();
        }

        public override WordModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, WordDto dto)
        {
            throw new NotImplementedException();
        }
    }
}