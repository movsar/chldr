using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_utils;

namespace chldr_shared.Services
{
    public class TranslationService : IDboService<TranslationModel, TranslationDto>
    {
        private readonly IDataProvider _dataProvider;
        private readonly RequestService _requestService;
        private readonly ExceptionHandler _exceptionHandler;

        public TranslationService(IDataProvider dataProvider, RequestService requestService, ExceptionHandler exceptionHandler)
        {
            _dataProvider = dataProvider;
            _requestService = requestService;
            _exceptionHandler = exceptionHandler;
        }

        public Task AddAsync(TranslationDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<TranslationModel> GetAsync(string translationId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            return await unitOfWork.Translations.GetAsync(translationId);
        }

        public Task RemoveAsync(TranslationModel entry, string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TranslationDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task PromoteAsync(ITranslation translationInfo, UserModel? currentUser)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.UserId);
            await unitOfWork.Translations.Promote(translationInfo);
        }

    }
}
