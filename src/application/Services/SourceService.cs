using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;
using domain.Interfaces;
using domain.Services;
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
