using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;

namespace chldr_shared.Services
{
    public class TranslationService
    {
        private readonly IDataProvider _dataProvider;

        public TranslationService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<TranslationModel> GetAsync(string translationId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            return await unitOfWork.Translations.GetAsync(translationId);
        }
        public async Task AddAsync(TranslationDto translationDto, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Translations.Add(translationDto);
        }

        public async Task RemoveAsync(string translationId, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Translations.Remove(translationId);
        }

        internal async Task PromoteAsync(ITranslation translationInfo, UserModel? currentUser)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.UserId);
            await unitOfWork.Translations.Promote(translationInfo);
        }
    }
}
