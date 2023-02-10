using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using Realms.Sync;
using System.Diagnostics;

namespace chldr_data.Services
{
    public class SyncedDataAccess : DataAccess, ISyncedDataAccess
    {
        #region Properties
        private new SyncedRealmService _realmService;

        public SyncedDataAccess(IRealmServiceFactory realmServiceFactory, UserService userService, ExceptionHandler exceptionHandler, NetworkService networkService) : base(realmServiceFactory, exceptionHandler, networkService)
        {
            _realmService = (realmServiceFactory.GetInstance(DataAccessType.Synced) as SyncedRealmService)!;
        }

        public App App => _realmService.GetApp();

        public override Realm Database => _realmService.GetDatabase();
        #endregion

        #region Events
        public event Action DatabaseSynchronized;
        #endregion


        #region DB Initializaion Related

        public async Task DatabaseMaintenance()
        {
            await Database.SyncSession.WaitForDownloadAsync();
            await Database.SyncSession.WaitForUploadAsync();

            try
            {
                var sources = Database.All<Source>().ToList();
                var unverifiedSources = SourcesRepository.GetUnverifiedSources();

                Database.Write(() =>
                {

                });

            }
            catch (Exception ex)
            {
                _exceptionHandler.ProcessError(ex);
            }
        }

        #endregion
    }
}
