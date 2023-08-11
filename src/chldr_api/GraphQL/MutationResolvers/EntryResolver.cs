using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
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

        //private async Task<bool> ValidateParent(EntryDto entryDto)
        //{
        //    // If Parent is specified, restrict to one level deep, parent child relationship, no hierarchies
        //    if (entryDto.ParentEntryId != null)
        //    {
        //        var parent = await GetAsync(entryDto.ParentEntryId);
        //        if (!string.IsNullOrEmpty(parent.ParentEntryId))
        //        {
        //            return false;
        //        }

        //        var children = await GetChildEntriesAsync(entryDto.EntryId);
        //        if (children.Count() > 0)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        public async Task<RequestResult> AddEntryAsync(string userId, EntryDto entryDto)
        {
            // UserId instead of object used for security reasons

            // TODO: Check permissions
            // TODO: Validate input

            //if (!(await ValidateParent(newEntryDto)))
            //{
            //    throw new InvalidArgumentsException("Error:Invalid_parent_entry");
            //}

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

        public async Task<RequestResult> UpdateEntry(string userId, EntryDto entryDto)
        {
            // UserId instead of object used for security reasons

            // TODO: Check permissions
            // TODO: Validate input

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

        public async Task<RequestResult> RemoveEntry(string userId, string entryId)
        {
            // UserId instead of object used for security reasons

            // TODO: Check permissions
            // TODO: Validate input

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
