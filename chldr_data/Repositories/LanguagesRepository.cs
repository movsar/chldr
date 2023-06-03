﻿
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class LanguagesRepository : Repository<SqlLanguage, LanguageModel, LanguageDto>
    {
        public LanguagesRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType => RecordType.Language;

        public override IEnumerable<ChangeSetModel> Add(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }

        public override LanguageModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
