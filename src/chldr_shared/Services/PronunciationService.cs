using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;

namespace chldr_shared.Services
{
    public class PronunciationService
    {
        private readonly IDataProvider _dataProvider;

        public PronunciationService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task RemoveAsync(string soundId, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Sounds.Remove(soundId);
        }

        internal async Task PromoteAsync(ISound soundInfo, UserModel? currentUser)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.UserId);
            await unitOfWork.Sounds.Promote(soundInfo);
        }
    }
}
