using chldr_data.Factories;
using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Repositories
{
    public abstract class Repository
    {
        public IDataAccess DataAccess { get; }
        public Realm Database => DataAccess.GetActiveDataservice().GetDatabase();
        public Repository(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        //TModel Find<TModel>(ObjectId Id) where TModel : IModelBase;
        //void Add<TModel>(TModel model) where TModel : IModelBase;
        //void Delete<TModel>(TModel model) where TModel : IModelBase;
        //void Update<TModel>(TModel model) where TModel : IModelBase;
    }
}