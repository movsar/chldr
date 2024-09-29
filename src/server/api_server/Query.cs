using core.DatabaseObjects.Models;
using core.Enums;
using core.Interfaces;
using core.Models;
using core.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace api_server
{
    public class Query
    {
        /*
         * Returns models, as we need to serialize data and there is no point 
         * in converting models first to DTOs if they too are going to be serialized.
         * By doing this we save on unecessary conversions and increase performance
         */
        private readonly IDataProvider _dataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Query(IDataProvider dataProvider, IHttpContextAccessor httpContextAccessor)
        {
            _dataProvider = dataProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        private void ValidateProtectedAccess()
        {
            // Access the HTTP headers
            var headers = _httpContextAccessor.HttpContext.Request.Headers;
            string apiKey = headers["ApiKey"];

            if (string.IsNullOrEmpty(apiKey) || apiKey != "Whatever")
            {
                throw new UnauthorizedAccessException("Invalid API Key");
            }
        }

        public async Task<RequestResult> CountAsync(string recordTypeName, FiltrationFlags? filtrationFlags)
        {
            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
            var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);

            int count = 0;

            switch (recordType)
            {
                case RecordType.Entry:
                    count = await unitOfWork.Entries.CountAsync(filtrationFlags);
                    break;
            }
            return new RequestResult()
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(count)
            };
        }

        public async Task<RequestResult> GetLatestEntriesAsync(int count)
        {
            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
            var foundEntries = await unitOfWork.Entries.GetLatestEntriesAsync(count);
            return new RequestResult()
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(foundEntries)
            };
        }

        public async Task<RequestResult> GetRandomEntriesAsync(int count)
        {
            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
            var foundEntries = await unitOfWork.Entries.GetRandomsAsync(count);
            return new RequestResult()
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(foundEntries)
            };
        }

        public async Task<RequestResult> GetEntriesAsync(List<string> entryIds, FiltrationFlags? filtrationFlags)
        {
            try
            {
                using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
                var foundEntries = await unitOfWork.Entries.GetByIdsAsync(entryIds, filtrationFlags);
                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(foundEntries)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new RequestResult()
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    SerializedData = string.Empty
                };
            }
        }

        public async Task<RequestResult> FindAsync(string inputText, FiltrationFlags? filtrationFlags)
        {
            try
            {
                var query = inputText.Replace("1", "Ӏ").ToLower();

                using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();
                var foundEntries = await unitOfWork.Entries.FindAsync(query, filtrationFlags);
                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(foundEntries)
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

        public async Task<RequestResult> TakeAsync(string recordTypeName, int offset, int limit, FiltrationFlags? filtrationFlags)
        {
            try
            {
                ValidateProtectedAccess();
                using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();

                object? models = null;
                var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);

                switch (recordType)
                {
                    case RecordType.User:
                        models = await unitOfWork.Users.TakeAsync(offset, limit);
                        break;

                    case RecordType.Source:
                        models = await unitOfWork.Sources.TakeAsync(offset, limit);
                        break;

                    case RecordType.Entry:
                        models = await unitOfWork.Entries.TakeAsync(offset, limit, filtrationFlags);
                        break;

                    case RecordType.ChangeSet:
                        models = await unitOfWork.ChangeSets.TakeAsync(offset, limit);
                        break;


                    default:
                        break;
                }
                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(models)
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
        public async Task<RequestResult> TakeLastAsync(string recordTypeName, int count, FiltrationFlags? filtrationFlags)
        {
            try
            {
                ValidateProtectedAccess();
                using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();

                object? models = null;
                var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);

                switch (recordType)
                {
                    case RecordType.ChangeSet:

                        List<ChangeSetModel> changeSets = null!;
                        models = await unitOfWork.ChangeSets.TakeLastAsync(count);


                        break;

                    default:
                        break;
                }

                return new RequestResult()
                {
                    Success = true,
                    SerializedData = JsonConvert.SerializeObject(models)
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