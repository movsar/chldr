using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_utils;

namespace dosham.Services
{
    public class SourceService : IDboService<SourceModel, SourceDto>
    {
        private readonly IDataProvider _dataProvider;
        private readonly RequestService _requestService;
        private readonly ExceptionHandler _exceptionHandler;
        public SourceService(IDataProvider dataProvider, RequestService requestService, ExceptionHandler exceptionHandler)
        {
            _dataProvider = dataProvider;
            _requestService = requestService;
            _exceptionHandler = exceptionHandler;
        }
       
        public Task AddAsync(SourceDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<SourceModel> GetAsync(string entryId)
        {
            throw new NotImplementedException();
        }

        public async Task<SourceModel> GetRandomSource()
        {
            var unitOfWork = _dataProvider.Repositories();
            var randomSources = await unitOfWork.Sources.GetRandomsAsync(1);
            return randomSources.First();
        }

        public Task RemoveAsync(SourceModel entry, string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(SourceDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
