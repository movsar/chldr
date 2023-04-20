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
    }
}