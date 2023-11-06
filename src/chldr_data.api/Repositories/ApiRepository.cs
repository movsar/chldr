using chldr_utils;
using chldr_data.Enums;
using chldr_utils.Services;
using chldr_data.realm.Interfaces;
using chldr_data.Services;

namespace chldr_data.Repositories
{
    public abstract class ApiRepository<TModel, TDto> : IApiRepository<TModel, TDto>
        where TDto : class, new()
        where TModel : class
    {
        protected abstract RecordType RecordType { get; }
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly FileService _fileService;
        protected readonly RequestService _requestService;

        public ApiRepository(
            ExceptionHandler exceptionHandler, 
            FileService fileService,
            RequestService requestService)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
            _requestService = requestService;
        }
        public abstract Task Add(TDto dto);
        public abstract Task Update(TDto EntryDto);
        public abstract Task Remove(string entityId);

        public async Task AddRange(IEnumerable<TDto> added)
        {
            foreach (var dto in added)
            {
                await Add(dto);
            }
        }
        public async Task UpdateRange(IEnumerable<TDto> updated)
        {
            foreach (var dto in updated)
            {
                await Update(dto);

            }
        }
        public async Task RemoveRange(IEnumerable<string> removed)
        {         
            foreach (var id in removed)
            {
                await Remove(id);
            }
        }

        public Task<List<TModel>> TakeAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<List<TModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetAsync(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}