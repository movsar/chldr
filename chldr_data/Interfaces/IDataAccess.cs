using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Repositories;
using chldr_data.Services;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        event Action DataAccessInitialized;
        Realm GetDatabase();
        void RemoveAllEntries();
        Repository GetRepository<T>() where T : IEntity;
        void InitializeDataSource();

        IGraphQLRequestSender RequestSender { get; set; }

    }
}