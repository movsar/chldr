using chldr_data.Factories;
using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Repositories
{
    public class Repository
    {
        private IRealmServiceFactory _realmServiceFactory;
        protected Realm Database => _realmServiceFactory.GetActiveInstance().GetDatabase();
        public Repository(IRealmServiceFactory realmServiceFactory)
        {
            _realmServiceFactory = realmServiceFactory;
        }

    }
}