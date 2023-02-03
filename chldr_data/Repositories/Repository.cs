using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
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