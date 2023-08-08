using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_data.local.Repositories;
using chldr_data.local.Services;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.Repositories;
using chldr_data.Responses;
using chldr_data.Services;
using chldr_utils;

namespace chldr_shared.Services
{
    public class EntryService
    {
        private readonly IDataProvider _dataProvider;
        private readonly RequestService _requestService;
        private readonly ExceptionHandler _exceptionHandler;

        public EntryService(IDataProvider dataProvider, RequestService requestService, ExceptionHandler exceptionHandler)
        {
            _dataProvider = dataProvider;
            _requestService = requestService;
            _exceptionHandler = exceptionHandler;
        }

        public async Task<EntryModel> GetAsync(string entryId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            return await unitOfWork.Entries.GetAsync(entryId);
        }

        #region Add
        private async Task AddToLocalDatabase(EntryDto newEntryDto, string userId, IEnumerable<ChangeSetDto> changeSets)
        {
            var unitOfWork = (RealmUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            var entriesRepository = (RealmEntriesRepository)unitOfWork.Entries;
            var changeSetsRepository = (RealmChangeSetsRepository)unitOfWork.ChangeSets;

            await entriesRepository.Add(newEntryDto);
            await changeSetsRepository.AddRange(changeSets);
        }

        private async Task<InsertResponse> AddToRemmoteDatabase(EntryDto newEntryDto, string userId)
        {
            if (newEntryDto == null || string.IsNullOrEmpty(newEntryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            // Insert remote entity with translations
            var response = await _requestService.AddEntry(userId, newEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            var responseData = RequestResult.GetData<InsertResponse>(response);
            if (responseData.CreatedAt == DateTimeOffset.MinValue)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            return responseData;
        }

        public async Task AddAsync(EntryDto entryDto, string userId)
        {
            try
            {
                var insertResponse = await AddToRemmoteDatabase(entryDto, userId);

                if (_dataProvider is SqlDataProvider)
                {
                    return;
                }

                await AddToLocalDatabase(entryDto, userId, insertResponse.ChangeSets);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Update
        private async Task UpdateInLocalDatabase(EntryDto newEntryDto, string userId, IEnumerable<ChangeSetDto> changeSets)
        {
            var unitOfWork = (RealmUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            var entriesRepository = (RealmEntriesRepository)unitOfWork.Entries;
            var changeSetsRepository = (RealmChangeSetsRepository)unitOfWork.ChangeSets;

            await entriesRepository.Update(newEntryDto);
            await changeSetsRepository.AddRange(changeSets);
        }

        private async Task<UpdateResponse> UpdateInRemmoteDatabase(EntryDto updatedEntryDto, string userId)
        {
            var response = await _requestService.UpdateEntry(userId, updatedEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);

            return responseData;
        }

        public async Task UpdateAsync(EntryDto entryDto, string userId)
        {
            try
            {
                var updateResponse = await UpdateInRemmoteDatabase(entryDto, userId);

                if (_dataProvider is SqlDataProvider)
                {
                    return;
                }

                await UpdateInLocalDatabase(entryDto, userId, updateResponse.ChangeSets);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Remove
        private async Task<UpdateResponse> RemoveFromRemmoteDatabase(EntryModel entry, string userId)
        {
            var response = await _requestService.RemoveEntry(userId, entry.EntryId);
            if (!response.Success)
            {
                throw _exceptionHandler.Error("Error:Request_failed");
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);
            return responseData;
        }
        private async Task RemoveFromLocalDatabase(EntryModel entry, string userId, IEnumerable<ChangeSetDto> changeSets)
        {
            var unitOfWork = (RealmUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            var entriesRepository = (RealmEntriesRepository)unitOfWork.Entries;
            var soundsRepository = (RealmSoundsRepository)unitOfWork.Sounds;
            var translationsRepository = (RealmTranslationsRepository)unitOfWork.Translations;
            var changeSetsRepository = (RealmChangeSetsRepository)unitOfWork.ChangeSets;

            var sounds = entry.Sounds.Select(s => s.SoundId).ToArray();
            var translations = entry.Translations.Select(t => t.TranslationId).ToArray();

            await soundsRepository.RemoveRange(sounds);
            await translationsRepository.RemoveRange(translations);
            await entriesRepository.Remove(entry.EntryId);

            await changeSetsRepository.AddRange(changeSets);
        }
        public async Task RemoveAsync(EntryModel entry, string userId)
        {
            try
            {
                var updateResponse = await RemoveFromRemmoteDatabase(entry, userId);

                if (_dataProvider is SqlDataProvider)
                {
                    return;
                }

                await RemoveFromLocalDatabase(entry, userId, updateResponse.ChangeSets);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        internal async Task PromoteAsync(IEntry entry, UserModel? currentUser)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.UserId);
            await unitOfWork.Entries.Promote(entry);
        }

        internal async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var entries = await unitOfWork.Entries.TakeAsync(offset, limit, filtrationFlags);
            return entries.ToList();
        }

        internal async Task<int> GetCountAsync(FiltrationFlags filtrationFlags)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            return await unitOfWork.Entries.CountAsync(filtrationFlags);
        }
    }
}
