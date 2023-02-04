using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Repositories
{
    public class Repository
    {
        private IRealmService _realmService;
        protected Realm Database => _realmService.GetDatabase();
        public Repository(IRealmService realmService)
        {
            _realmService = realmService;
        }

    }
}