using api_dl.ApiDataProvider.Interfaces;
using domain;
using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using domain;
using domain.Interfaces;
using domain.Interfaces.Repositories;
using domain.Models;
using domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using domain.Enums;

namespace api_dl.ApiDataProvider.Repositories
{
    internal class ApiEntriesRepository : ApiRepository<EntryModel, EntryDto>, IEntriesRepository
    {
        public ApiEntriesRepository(IRequestService requestService) : base(requestService) { }

        public override Task Add(EntryDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountAsync(FiltrationFlags? filtrationFlags = null)
        {
            var response = await _requestService.CountAsync(RecordType.Entry, filtrationFlags);
            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }
            var entriesCount = RequestResult.GetData<int>(response);
            return entriesCount;
        }

        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags? filtrationFlags = null)
        {
            var response = await _requestService.FindAsync(inputText, filtrationFlags);
            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }
            var entries = RequestResult.GetData<List<EntryModel>>(response);
            return entries;
        }

        public EntryModel FromEntry(string entryId)
        {
            throw new NotImplementedException();
        }

        public override Task<EntryModel> GetAsync(string entityId)
        {
            throw new NotImplementedException();
        }

        public Task<List<EntryModel>> GetByIdsAsync(List<string> entryIds, FiltrationFlags? filtrationFlags = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<EntryModel>> GetEntriesOnModerationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<EntryModel>> GetLatestEntriesAsync(int count)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<EntryModel>> GetRandomsAsync(int limit)
        {
            var entries = new List<EntryModel>();
            var response = await _requestService.GetRandomsAsync(limit);
            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }
            entries = RequestResult.GetData<List<EntryModel>>(response);
            return entries;
        }

        public Task<ChangeSetModel> Promote(IEntry entry)
        {
            throw new NotImplementedException();
        }

        public override Task Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags? filtrationFlags = null)
        {
            var response = await _requestService.TakeAsync(RecordType.Entry, offset, limit);
            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }
            var entries = RequestResult.GetData<List<EntryModel>>(response);
            return entries;
        }

        public override Task<List<EntryModel>> TakeAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Update(EntryDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
