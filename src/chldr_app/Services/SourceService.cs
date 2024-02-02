using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_utils;

namespace chldr_app.Services
{
    public class SourceService : IDboService<SourceModel, SourceDto>
    {
        private readonly IDataProvider _dataProvider;
        private readonly IRequestService _requestService;
        private readonly IExceptionHandler _exceptionHandler;
        public SourceService(IDataProvider dataProvider, IRequestService requestService, IExceptionHandler exceptionHandler)
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
