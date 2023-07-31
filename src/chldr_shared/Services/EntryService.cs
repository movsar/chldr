﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;

namespace chldr_shared.Services
{
    public class EntryService
    {
        private readonly IDataProvider _dataProvider;

        public EntryService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<EntryModel> Get(string entryId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            return await unitOfWork.Entries.GetAsync(entryId);
        }
        public async Task AddEntry(EntryDto entryDto, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Entries.Add(entryDto);
        }

        public async Task Update(EntryDto entryDto, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Entries.Update(entryDto);
        }

        public async Task Remove(string entryId, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Entries.Remove(entryId);
        }
    }
}
