using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.sql.Repositories;
using chldr_data.Responses;
using chldr_data.Services;
using chldr_utils;
using Newtonsoft.Json;
using chldr_data.Exceptions;

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
                var unitOfWork = _dataProvider.Repositories();
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
            var unitOfWork = _dataProvider.Repositories();

            if (entryDto == null || string.IsNullOrEmpty(entryDto.EntryId))
            {
                throw new NullReferenceException();
            }

            await CheckParentEntryId(entryDto);
        }
        private async Task CheckLoggedInUser(string userId)
        {
            var unitOfWork = _dataProvider.Repositories();
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

            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories(userId);
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
                        ChangeSets = changeSets,
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

            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories(userId);
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
                        ChangeSets = changeSets
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

            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories(userId);
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
                        ChangeSets = changeSets
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
            var unitOfWork = _dataProvider.Repositories();
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
            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories(userId);
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
                        ChangeSets = new List<ChangeSetModel>() { changeSet }
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

        internal async Task<RequestResult> AddSoundAsync(string currentUserId, PronunciationDto pronunciation)
        {
            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories(currentUserId);
            unitOfWork.BeginTransaction();
            var entriesRepository = (SqlEntriesRepository)unitOfWork.Entries;
            var soundsRepository = (SqlPronunciationsRepository)unitOfWork.Sounds;

            try
            {
                var changeSets = await soundsRepository.AddAsync(pronunciation);
                unitOfWork.Commit();

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(new UpdateResponse()
                    {
                        ChangeSets = changeSets
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
