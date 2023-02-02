using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    internal interface IRepository
    {
        void Add<TModel>(TModel model) where TModel : IModelBase;
        TModel Get<TModel>(ObjectId Id);
        void Delete<TModel>(TModel model) where TModel : IModelBase;
        void Update<TModel>(TModel model) where TModel : IModelBase;
    }
}
