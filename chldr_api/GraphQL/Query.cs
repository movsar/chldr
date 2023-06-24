﻿using chldr_data.DatabaseObjects.Dtos;
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
        protected readonly IConfiguration _configuration;

        public Query(
            IDataProvider dataProvider,
            IConfiguration configuration
            )
        {
            _dataProvider = dataProvider;
            _configuration = configuration;
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
                        var changeSets = await unitOfWork.ChangeSets.TakeLastAsync(count);
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