using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_tools;
using chldr_utils.Interfaces;
using chldr_utils;
using Realms;
using chldr_utils.Services;
using chldr_data.Models;
using System.Threading.Channels;
using chldr_data.Services;

namespace chldr_data.Repositories
{
    public class ApiSourcesRepository : ApiRepository< SourceModel, SourceDto>, ISourcesRepository
    {
        public ApiSourcesRepository(ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Source;


        public List<SourceModel> GetAllNamedSources()
        {
            throw new NotImplementedException();
        }
        public override async Task<List<ChangeSetModel>> Add(SourceDto dto)
        {
            throw new NotImplementedException();

        }
        public override async Task<List<ChangeSetModel>> Update(SourceDto dto)
        {
            throw new NotImplementedException();

        }

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
