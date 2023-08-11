using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.Responses;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace chldr_api.GraphQL.MutationResolvers
{
    public class EntryResolver
    {
        private readonly IDataProvider _dataProvider;
        private readonly ExceptionHandler _exceptionHandler;

        public EntryResolver(IDataProvider dataProvider, ExceptionHandler exceptionHandler)
        {
            _dataProvider = dataProvider;
            _exceptionHandler = exceptionHandler;
        }
        private async Task CheckParentEntryId(EntryDto entryDto)
        {
            // If Parent is specified, restrict to one level deep, parent child relationship, no hierarchies
            if (entryDto.ParentEntryId != null)
            {
                var unitOfWork = _dataProvider.CreateUnitOfWork();
                var entriesRepository = (SqlEntriesRepository)unitOfWork.Entries;

                var parent = await entriesRepository.GetAsync(entryDto.ParentEntryId);
                if (!string.IsNullOrEmpty(parent.ParentEntryId))
                {
                    throw new InvalidArgumentsException();
                }

                var children = await entriesRepository.GetChildEntriesAsync(entryDto.EntryId);
                if (children.Count() > 0)
                {
                    throw new InvalidArgumentsException();
                }
            }
        }
        private async Task CheckEntryDto(EntryDto entryDto)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();

            if (entryDto == null || string.IsNullOrEmpty(entryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            await CheckParentEntryId(entryDto);
        }
        private async Task CheckLoggedInUser(string userId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;

            var user = await usersRepository.GetAsync(userId);
            if (user.Status != UserStatus.Active || user.Rate < 1)
            {
                throw new UnauthorizedException();
            }
        }

        #region Add
        public async Task<RequestResult> AddEntryAsync(string userId, EntryDto entryDto)
        {
            // UserId instead of object used for security reasons
            await CheckLoggedInUser(userId);
            await CheckEntryDto(entryDto);

            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            var entriesRepository = (SqlEntriesRepository)unitOfWork.Entries;
            try
            {
                var changeSets = await entriesRepository.AddAsync(entryDto);
                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new InsertResponse
                    {
                        ChangeSets = changeSets.Select(ChangeSetDto.FromModel),
                        CreatedAt = entryDto.CreatedAt
                    })
                };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return new RequestResult();
        }
        #endregion

        #region Update
        private async Task CheckUpdatePermissions(EntryDto entryDto, string userId)
        {

        }

        public async Task<RequestResult> UpdateEntry(string userId, EntryDto entryDto)
        {
            // UserId instead of object used for security reasons

            await CheckLoggedInUser(userId);
            await CheckEntryDto(entryDto);
            await CheckUpdatePermissions(entryDto, userId);

            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            var entriesRepository = (SqlEntriesRepository)unitOfWork.Entries;
            try
            {
                var changeSets = await entriesRepository.UpdateAsync(entryDto);
                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new UpdateResponse()
                    {
                        ChangeSets = changeSets.Select(ChangeSetDto.FromModel)
                    })
                };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return new RequestResult();
        }

        #endregion
        public async Task<RequestResult> RemoveEntry(string userId, string entryId)
        {
            // UserId instead of object used for security reasons

            await CheckLoggedInUser(userId);
            await CheckRemovePermissions(userId, entryId);

            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            var entriesRepository = (SqlEntriesRepository)unitOfWork.Entries;
            try
            {
                var changeSets = await entriesRepository.RemoveAsync(entryId);
                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new UpdateResponse()
                    {
                        ChangeSets = changeSets.Select(ChangeSetDto.FromModel)
                    })
                };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new RequestResult();
        }

        private async Task CheckRemovePermissions(string userId, string entryId)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var entry = await unitOfWork.Entries.GetAsync(entryId);
            var user = await unitOfWork.Users.GetAsync(userId);

            // Check whether the user has access to remove the entry with all its child items
            if (!user.CanRemove(entry.Rate, entry.UserId, entry.CreatedAt))
            {
                throw new UnauthorizedException();
            }
            foreach (var item in entry.Translations)
            {
                if (!user.CanRemove(item.Rate, item.UserId, item.CreatedAt))
                {
                    throw new UnauthorizedException();
                }
            }
            foreach (var item in entry.Sounds)
            {
                if (!user.CanRemove(item.Rate, item.UserId, item.CreatedAt))
                {
                    throw new UnauthorizedException();
                }
            }
        }

        internal async Task<RequestResult> PromoteAsync(string userId, string entryId)
        {
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            var entriesRepository = (SqlEntriesRepository)unitOfWork.Entries;

            try
            {
                var entry = await entriesRepository.GetAsync(entryId);
                var changeSet = await entriesRepository.Promote(entry);
                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new UpdateResponse()
                    {
                        ChangeSets = new List<ChangeSetDto>() {
                            ChangeSetDto.FromModel(changeSet)
                        }
                    })
                };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
                return new RequestResult();
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
