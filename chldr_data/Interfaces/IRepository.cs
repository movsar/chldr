using MongoDB.Bson;

namespace chldr_data.Interfaces
{
    internal interface IRepository
    {
        TModel Find<TModel>(ObjectId Id) where TModel : IModelBase;
        void Add<TModel>(TModel model) where TModel : IModelBase;
        void Delete<TModel>(TModel model) where TModel : IModelBase;
        void Update<TModel>(TModel model) where TModel : IModelBase;
    }
}
