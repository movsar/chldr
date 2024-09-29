using chldr_utils;
using domain;
using Realms;
using chldr_utils.Services;
using realm_dl.Interfaces;
using domain.Interfaces;
using domain;
using domain.Enums;

namespace realm_dl.Repositories
{
    public abstract class RealmRepository<TEntity, TModel, TDto> : IRealmRepository<TModel, TDto>
        where TEntity : RealmObject, new()
        where TDto : class, new()
        where TModel : class
    {
        protected abstract RecordType RecordType { get; }
        private readonly RealmConfiguration _dbConfig;
        protected Realm _dbContext => Realm.GetInstance(_dbConfig);
        protected readonly IExceptionHandler _exceptionHandler;
        protected readonly IFileService _fileService;
        protected readonly string _userId;

        public RealmRepository(IExceptionHandler exceptionHandler, IFileService fileService, string userId)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;

            _userId = userId;

            _dbConfig = new RealmConfiguration(_fileService.DatabaseFilePath)
            {
                SchemaVersion = Constants.RealmSchemaVersion
            };
        }
        public abstract TModel FromEntity(TEntity entity);

        public abstract Task Add(TDto dto);
        public abstract Task Update(TDto EntryDto);
        public abstract Task Remove(string entityId);
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<TModel> GetAsync(string entityId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var entry = _dbContext.Find<TEntity>(entityId);
            if (entry == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(entry);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<List<TModel>> TakeAsync(int offset, int limit)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var entities = _dbContext.All<TEntity>()
                .Skip(offset)
                .Take(limit)
                .ToList();
            return entities.Select(FromEntity).ToList();
        }
        public virtual async Task<List<TModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();

            var entries = await Task.Run(() => _dbContext.All<TEntity>().AsEnumerable()
                  .OrderBy(x => randomizer.Next(0, Constants.EntriesApproximateCoount))
                  .OrderBy(entry => entry.GetHashCode())
                  .Take(limit)
                  .ToList()
                  .Select(FromEntity)
                  .ToList());

            return entries;
        }

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

    }
}