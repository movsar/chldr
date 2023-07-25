using chldr_utils;
using chldr_data.Enums;
using Realms;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using chldr_data.local.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Services;
using MySqlX.XDevAPI.Common;

namespace chldr_data.Repositories
{
    public abstract class RealmRepository<TEntity, TModel, TDto> : IRealmRepository<TModel, TDto>
        where TEntity : RealmObject, new()
        where TDto : class, new()
        where TModel : class
    {
        protected abstract RecordType RecordType { get; }
        private readonly RealmConfiguration _dbConfig;
        protected Realm _dbContext => Realm.GetInstance(_dbConfig);
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly FileService _fileService;
        protected readonly RequestService _requestService;
        protected readonly string _userId;

        public RealmRepository(ExceptionHandler exceptionHandler, FileService fileService, RequestService requestService, string userId)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
            _requestService = requestService;

            _userId = userId;

            _dbConfig = new RealmConfiguration(_fileService.OfflineDatabaseFilePath)
            {
                SchemaVersion = Constants.RealmSchemaVersion
            };
        }
        protected abstract TModel FromEntityShortcut(TEntity entity);

        public abstract Task<List<ChangeSetModel>> Add(TDto dto, string userId);
        public abstract Task<List<ChangeSetModel>> Update(TDto EntryDto, string userId);
        public async Task<TModel> Get(string entityId)
        {
            var entry = _dbContext.Find<TEntity>(entityId);
            if (entry == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntityShortcut(entry);
        }
        public virtual async Task<List<ChangeSetModel>> Remove(string entityId, string userId)
        {
            var entity = _dbContext.Find<TEntity>(entityId);
            if (entity == null)
            {

                // ! NOT IMPLEMENTED
                return new List<ChangeSetModel>();
            }

            _dbContext.Write(() =>
            {
                _dbContext.Remove(entity);
            });

            if (RecordType == RecordType.ChangeSet)
            {
                // ! NOT IMPLEMENTED
                return new List<ChangeSetModel>();
            }


            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public async Task<IEnumerable<TModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.All<TEntity>()
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return entities.Select(FromEntityShortcut).ToList();
        }
        public virtual async Task<List<TModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();

            var entries = await Task.Run(() => _dbContext.All<TEntity>().AsEnumerable()
                  .OrderBy(x => randomizer.Next(0, 75000))
                  .OrderBy(entry => entry.GetHashCode())
                  .Take(limit)
                  .Select(FromEntityShortcut)
                  .ToList());

            return entries;
        }

        public async Task<List<ChangeSetModel>> AddRange(IEnumerable<TDto> added, string userId)
        {
            var result = new List<ChangeSetModel>();
            foreach (var dto in added)
            {
                result.AddRange(await Add(dto, userId));
            }

            return result;
        }
        public async Task<List<ChangeSetModel>> UpdateRange(IEnumerable<TDto> updated, string userId)
        {
            var result = new List<ChangeSetModel>();
            foreach (var dto in updated)
            {
                result.AddRange(await Update(dto, userId));

            }

            return result;
        }
        public async Task<List<ChangeSetModel>> RemoveRange(IEnumerable<string> removed, string userId)
        {
            var result = new List<ChangeSetModel>();
            foreach (var id in removed)
            {
                result.AddRange(await Remove(id, userId));
            }

            return result;
        }

    }
}