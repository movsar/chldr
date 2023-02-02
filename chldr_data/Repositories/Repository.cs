using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Realms;
using System.Collections.Generic;
using System.Text.Json;
using chldr_data.Interfaces;
using MongoDB.Bson;
using chldr_data.Entities;
using Realms.Exceptions;

namespace Data.Repositories
{
    public class Repository<TEntity> : IRepository where TEntity : RealmObject, new()
    {
        private readonly IRealmService _realmService;
        internal Repository(IRealmService realmService)
        {
            _realmService = realmService;
        }

        #region Generic CRUD

        public TModel Get<TModel>(ObjectId id)
        {
            var result = _realmService.GetDatabase().Find<TEntity>(id);


            return EntitiesToModels<TEntity, TModel>(result);
        }

        public virtual void Add<TModel>(TModel model) where TModel : IModelBase
        {
            dynamic entity = new TEntity();
            entity.SetFromModel(model);

            _realmService.GetDatabase().Write(() =>
            {
                _realmService.GetDatabase().Add(entity);
            });

            // Set the Id for the inserted object
            model.Id = entity.Id;
        }

        public virtual void Update<TModel>(TModel model) where TModel : IModelBase
        {
            dynamic entity = _realmService.GetDatabase().Find<TEntity>(model.Id);
            _realmService.GetDatabase().Write(() =>
            {
                entity.SetFromModel(model);
            });
        }

        public virtual void Delete<TModel>(TModel model) where TModel : IModelBase
        {
            var entity = _realmService.GetDatabase().Find<TEntity>(model.Id);
            _realmService.GetDatabase().Write(() =>
            {
                _realmService.GetDatabase().Remove(entity);
            });
        }

        public virtual IEnumerable<TModel> GetAll<TModel>() where TModel : IModelBase
        {
            var entries = _realmService.GetDatabase().All<TEntity>();
            return EntitiesToModels<TEntity, TModel>(entries);
        }

        #endregion

        #region EntitiesToModels

        // These method takes RealmObjects and turns them into plain model objects, works only for retrieval

        internal IEnumerable<TTarget> EntitiesToModels<TSource, TTarget>(IEnumerable<TSource> realmObjects)
        {
            string jsonString = JsonSerializer.Serialize(realmObjects);
            return JsonSerializer.Deserialize<IEnumerable<TTarget>>(jsonString);
        }
        public TTarget EntitiesToModels<TSource, TTarget>(TSource realmObject)
        {
            string jsonString = JsonSerializer.Serialize(realmObject);
            return JsonSerializer.Deserialize<TTarget>(jsonString);
        }

        #endregion

    }
}

