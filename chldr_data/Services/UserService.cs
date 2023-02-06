using chldr_data.Enums;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_utils;
using chldr_utils.Services;
using MongoDB.Bson;
using Realms;
using Realms.Sync;

namespace chldr_data.Services
{
    public class UserService
    {
        public event Action<UserModel, SessionStatus>? UserStateHasChanged;
        private readonly NetworkService _networkService;
        private readonly IRealmServiceFactory _realmServiceFactory;
        private readonly SyncedRealmService _realmService;

        private App App => _realmService.GetApp();
        private Realm Database => _realmService.GetDatabase();
        public UserService(NetworkService networkService, IRealmServiceFactory realmServiceFactory)
        {
            _networkService = networkService;
            _realmServiceFactory = realmServiceFactory;
            _realmService = (realmServiceFactory.GetInstance(DataAccessType.Synced) as SyncedRealmService)!;
            _realmService.DatasourceInitialized += RealmService_DatasourceInitialized;
        }
        private void RealmService_DatasourceInitialized()
        {
            if (DataAccess.CurrentDataAccess != DataAccessType.Synced)
            {
                return;
            }

            if (App?.CurrentUser?.Provider == Credentials.AuthProvider.Anonymous)
            {
                return;
            }

            var sessionStatus = GetCurrentUserSessionStatus();
            var userInfo = GetCurrentUserInfo();

            UserStateHasChanged?.Invoke(userInfo, sessionStatus);
        }

        private SessionStatus GetCurrentUserSessionStatus()
        {
            if (!_networkService.IsNetworUp || Database.SyncSession.State == SessionState.Inactive)
            {
                return SessionStatus.Disconnected;
            }

            if (App?.CurrentUser?.Id == null)
            {
                return SessionStatus.Unauthorized;
            }

            if (App.CurrentUser.Provider == Credentials.AuthProvider.Anonymous)
            {
                return SessionStatus.Unauthorized;
            }

            return SessionStatus.LoggedIn;
        }

        public UserModel GetCurrentUserInfo()
        {
            var appUserId = new ObjectId(App.CurrentUser.Id);
            var user = Database.All<Entities.User>().FirstOrDefault(u => u._id == appUserId);

            if (user == null)
            {
                throw new Exception(AppConstants.DataErrorMessages.NoUserInfo);
            }

            return new UserModel(user);
        }

        public async Task RegisterNewUserAsync(string email, string password)
        {
            await App.EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await App.EmailPasswordAuth.CallResetPasswordFunctionAsync(email, "somerandomsuperhardpassword");
        }

        public async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await App.EmailPasswordAuth.ResetPasswordAsync(newPassword, token, tokenId);
        }

        public async Task ConfirmUserAsync(string token, string tokenId, string userEmail)
        {
            await App.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            await App.LogInAsync(Credentials.EmailPassword(email, password));
            _realmService.InitializeDataSource();
        }

        public async Task LogOutAsync()
        {
            var defaultUser = App.AllUsers.FirstOrDefault(u => u.Provider == Credentials.AuthProvider.Anonymous);
            if (defaultUser != null)
            {
                App.SwitchUser(defaultUser);
            }

            DataAccess.CurrentDataAccess = DataAccessType.Offline;
            var offlineRealmService = (_realmServiceFactory.GetInstance(DataAccessType.Offline) as OfflineRealmService)!;
            offlineRealmService.InitializeDataSource();
        }
    }
}
