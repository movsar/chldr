using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.Responses;
using chldr_data.Services;
using chldr_utils;
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
        public async Task<RequestResult> AddEntryAsync(string userId, EntryDto entryDto)
        {
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var changeSets = await unitOfWork.Entries.Add(entryDto);
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
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var changeSets = await unitOfWork.Entries.Update(entryDto);
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
            using var unitOfWork = (SqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();
            try
            {
                var changeSets = await unitOfWork.Entries.Remove(entryId);
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

    }
}
