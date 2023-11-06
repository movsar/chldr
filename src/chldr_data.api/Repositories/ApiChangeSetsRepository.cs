using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_tools;
using chldr_utils;
using Realms;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using chldr_data.Services;

namespace chldr_data.Repositories
{
    public class ApiChangeSetsRepository : ApiRepository<ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        public ApiChangeSetsRepository(ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.ChangeSet;
   
        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<ChangeSetModel>> Update(ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override async Task<List<ChangeSetModel>> Add(ChangeSetDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ChangeSetModel>> TakeLastAsync(int count)
        {
            throw new NotImplementedException();
        }

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
