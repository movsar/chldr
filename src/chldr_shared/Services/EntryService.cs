﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
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

        public async Task<EntryModel> GetAsync(string entryId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(null);
            return await unitOfWork.Entries.GetAsync(entryId);
        }
        public async Task AddAsync(EntryDto entryDto, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Entries.Add(entryDto);
        }

        public async Task UpdateAsync(EntryDto entryDto, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Entries.Update(entryDto);
        }

        public async Task RemoveAsync(string entryId, string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(userId);
            await unitOfWork.Entries.Remove(entryId);
        }

        internal async Task PromoteAsync(IEntry entry, UserModel? currentUser)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork(currentUser.UserId);
            await unitOfWork.Entries.Promote(entry);
        }
    }
}
