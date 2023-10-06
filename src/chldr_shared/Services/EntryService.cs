using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
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
using Microsoft.AspNetCore.Http;
using Realms.Sync;

namespace chldr_shared.Services
{
    public class EntryService : IDboService<EntryModel, EntryDto>
    {
        internal event Func<EntryModel, Task> EntryUpdated;
        internal event Func<EntryModel, Task> EntryInserted;
        internal event Func<EntryModel, Task> EntryRemoved;
        internal event Action<SearchResultModel>? NewDeferredSearchResult;

        private readonly IDataProvider _dataProvider;
        private readonly RequestService _requestService;
        private readonly ExceptionHandler _exceptionHandler;

        public EntryService(
            IDataProvider dataProvider, 
            RequestService requestService, 
            ExceptionHandler exceptionHandler)
        {
            _dataProvider = dataProvider;
            _requestService = requestService;
            _exceptionHandler = exceptionHandler;
        }

        #region Get
        public async Task<List<EntryModel>> FindAsync(string inputText)
        {
            var query = inputText.Replace("1", "Ӏ").ToLower();

            var unitOfWork = _dataProvider.CreateUnitOfWork();
            return (await unitOfWork.Entries.FindAsync(query)).ToList();
        }

        public async Task FindDeferredAsync(string inputText)
        {
            var result = new List<EntryModel>();

            await Task.Run(async () =>
            {
                result = await FindAsync(inputText);
            });

            var args = new SearchResultModel(inputText, result, SearchResultModel.Mode.Full);
            NewDeferredSearchResult?.Invoke(args);
        }

        public async Task<EntryModel> GetAsync(string entryId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            return await unitOfWork.Entries.GetAsync(entryId);
        }
        #endregion

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
            var response = await _requestService.AddEntryAsync(newEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            var responseData = RequestResult.GetData<InsertResponse>(response);
            if (responseData.CreatedAt == DateTimeOffset.MinValue)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            return responseData;
        }

        public async Task AddAsync(EntryDto entryDto, string userId)
        {
            try
            {
                var insertResponse = await AddToRemmoteDatabase(entryDto, userId);

                if (_dataProvider is RealmDataProvider)
                {
                    await AddToLocalDatabase(entryDto, userId, insertResponse.ChangeSets);
                }

                var unitOfWork = _dataProvider.CreateUnitOfWork();
                var entry = await unitOfWork.Entries.GetAsync(entryDto.EntryId);
                EntryInserted?.Invoke(entry);
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
            var response = await _requestService.UpdateEntry(updatedEntryDto);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);

            return responseData;
        }

        public async Task UpdateAsync(EntryDto entryDto, string userId)
        {
            try
            {
                var updateResponse = await UpdateInRemmoteDatabase(entryDto, userId);

                if (_dataProvider is RealmDataProvider)
                {
                    await UpdateInLocalDatabase(entryDto, userId, updateResponse.ChangeSets);
                }

                var unitOfWork = _dataProvider.CreateUnitOfWork();
                var updatedEntry = await unitOfWork.Entries.GetAsync(entryDto.EntryId);
                EntryUpdated?.Invoke(updatedEntry);
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
            var response = await _requestService.RemoveEntry(entry.EntryId);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
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

                if (_dataProvider is RealmDataProvider)
                {
                    await RemoveFromLocalDatabase(entry, userId, updateResponse.ChangeSets);
                }

                EntryRemoved?.Invoke(entry);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        private async Task<UpdateResponse> PromoteRequestAsync(IEntry entry, UserModel? currentUser)
        {
            var response = await _requestService.PromoteAsync(RecordType.Entry, entry.EntryId);
            if (!response.Success)
            {
                throw _exceptionHandler.Error(response.ErrorMessage);
            }
            var responseData = RequestResult.GetData<UpdateResponse>(response);

            return responseData;
        }
        public async Task PromoteAsync(IEntry entry, UserModel? currentUser)
        {
            await PromoteRequestAsync(entry, currentUser);

            // TODO: Upate local, if used

            //EntryUpdated?.Invoke(entry);
        }

        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var entries = await unitOfWork.Entries.TakeAsync(offset, limit, filtrationFlags);
            return entries.ToList();
        }

        public async Task<int> GetCountAsync(FiltrationFlags filtrationFlags)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            return await unitOfWork.Entries.CountAsync(filtrationFlags);
        }

        public async Task PromoteTranslationAsync(ITranslation translationInfo, UserModel? currentUser)
        {
            // TODO: Use requestService
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.Id);
            await unitOfWork.Translations.Promote(translationInfo);
        }

        public async Task PromotePronunciationAsync(IPronunciation soundInfo, UserModel? currentUser)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.Id);
            await unitOfWork.Sounds.Promote(soundInfo);
        }
    }
}
