using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_utils;

namespace chldr_shared.Services
{
    public class PronunciationService : IDboService<PronunciationModel, PronunciationDto>
    {
        private readonly IDataProvider _dataProvider;
        private readonly RequestService _requestService;
        private readonly ExceptionHandler _exceptionHandler;

        public PronunciationService(IDataProvider dataProvider, RequestService requestService, ExceptionHandler exceptionHandler)
        {
            _dataProvider = dataProvider;
            _requestService = requestService;
            _exceptionHandler = exceptionHandler;
        }

        public Task AddAsync(PronunciationDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<PronunciationModel> GetAsync(string entryId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(string soundId, string userId)
        {
        }

        public Task RemoveAsync(PronunciationModel entry, string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(PronunciationDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<SourceModel> GetRandomSource()
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var randomSources = await unitOfWork.Sources.GetRandomsAsync(1);
            return randomSources.First();
        }

        public async Task PromoteAsync(ISound soundInfo, UserModel? currentUser)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.UserId);
            await unitOfWork.Sounds.Promote(soundInfo);
        }
    }
}
