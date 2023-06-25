using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.ResponseTypes;
using Newtonsoft.Json;
using Realms.Sync;

namespace chldr_api
{
    public class Query
    {
        private readonly IDataProvider _dataProvider;

        public Query(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<RequestResult> TakeAsync(string recordTypeName, int offset, int limit)
        {
            try
            {
                using var unitOfWork = (ISqlUnitOfWork)_dataProvider.CreateUnitOfWork();

                object? dtos = null;
                var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);

                switch (recordType)
                {
                    case RecordType.User:
                        var users = await unitOfWork.Users.TakeAsync(offset, limit);
                        dtos = users.Select(UserDto.FromModel);
                        break;

                    case RecordType.Source:
                        var sources = await unitOfWork.Sources.TakeAsync(offset, limit);
                        dtos = sources.Select(SourceDto.FromModel);
                        break;

                    case RecordType.Entry:
                        var entries = await unitOfWork.Entries.TakeAsync(offset, limit);
                        dtos = entries.Select(EntryDto.FromModel);
                        break;

                    case RecordType.ChangeSet:
                        var changeSets = await unitOfWork.ChangeSets.TakeAsync(offset, limit);
                        dtos = changeSets.Select(ChangeSetDto.FromModel);
                        break;


                    default:
                        break;
                }

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(dtos)
                };
            }
            catch (Exception ex)
            {
                return new RequestResult()
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    SerializedData = string.Empty
                };
            }
        }

        public async Task<RequestResult> TakeLastAsync(string recordTypeName, int count)
        {
            try
            {
                using var unitOfWork = (ISqlUnitOfWork)_dataProvider.CreateUnitOfWork();

                object? dtos = null;
                var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);

                switch (recordType)
                {
                    case RecordType.ChangeSet:

                        List<ChangeSetModel> changeSets = null!;
                        try
                        {
                            changeSets = await unitOfWork.ChangeSets.TakeLastAsync(count);
                        }
                        catch (Exception ex)
                        {
                            // These errors get thrown occasionally, around 1/5
                            // 1. An item with the same key has already been added. Key: server=104.248.40.142;port=3306;..
                            // 2. Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.
                            throw;
                        }

                        dtos = changeSets.Select(ChangeSetDto.FromModel);

                        break;

                    default:
                        break;
                }

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(dtos)
                };
            }
            catch (Exception ex)
            {
                return new RequestResult()
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    SerializedData = string.Empty
                };
            }
        }
    }
}