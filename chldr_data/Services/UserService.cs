using chldr_data.Enums;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils;
using chldr_utils.Services;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using Realms.Sync;

namespace chldr_data.Services
{
    public class UserService
    {
        public event Action<IUser, SessionStatus>? UserStateHasChanged;
        private readonly NetworkService _networkService;
        private readonly IRealmServiceFactory _realmServiceFactory;
        private readonly AuthService _authService;
        private readonly SyncedRealmService _dataSourceService;

        private App App => _dataSourceService.GetApp();
        private Realm Database => _dataSourceService.GetDatabase();
        public UserService(NetworkService networkService, IRealmServiceFactory realmServiceFactory, AuthService authService)
        {
            _networkService = networkService;
            _realmServiceFactory = realmServiceFactory;
            _authService = authService;
            _dataSourceService = (realmServiceFactory.GetInstance(DataSourceType.Synced) as SyncedRealmService)!;
            _dataSourceService.DatasourceInitialized += RealmService_DatasourceInitialized;
        }
        private async void RealmService_DatasourceInitialized(DataSourceType dataSourceType)
        {
            if (dataSourceType != DataSourceType.Synced)
            {
                return;
            }

            if (App?.CurrentUser?.Provider == Credentials.AuthProvider.Anonymous)
            {
                return;
            }

            await Database.Subscriptions.WaitForSynchronizationAsync();
            await Database.SyncSession.WaitForDownloadAsync();

            var sessionStatus = GetCurrentUserSessionStatus();
            var userInfo = GetActiveSession();

            UserStateHasChanged?.Invoke(userInfo, sessionStatus);
        }

        private SessionStatus GetCurrentUserSessionStatus()
        {
            if (!_networkService.IsNetworUp || Database.SyncSession.State == SessionState.Inactive)
            {
                return SessionStatus.Offline;
            }

            if (App?.CurrentUser?.Id == null)
            {
                return SessionStatus.Anonymous;
            }

            if (App.CurrentUser.Provider == Credentials.AuthProvider.Anonymous)
            {
                return SessionStatus.Anonymous;
            }

            return SessionStatus.LoggedIn;
        }

        public UserModel GetActiveSession()
        {
            var appUserId = App.CurrentUser.Id;
            var user = Database.All<Entities.RealmUser>().FirstOrDefault(u => u.UserId == appUserId);

            if (user == null)
            {
                throw new Exception(AppConstants.DataErrorMessages.NoUserInfo);
            }

            return new UserModel(user);
        }

        public async Task<string> RegisterNewUserAsync(string email, string password)
        {
            return await _authService.RegisterUserAsync(email, password);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _authService.PasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await _authService.UpdatePasswordAsync(token, newPassword);
        }

        public async Task ConfirmUserAsync(string token, string tokenId, string userEmail)
        {
            await _authService.ConfirmUserAsync(token, tokenId, userEmail);
        }

        public async Task<ActiveSession> LogInEmailPasswordAsync(string email, string password)
        {
            return await _authService.LogInEmailPasswordAsync(email, password);
        }

        public void LogOutAsync()
        {
            var defaultUser = App.AllUsers.FirstOrDefault(u => u.Provider == Credentials.AuthProvider.Anonymous);
            if (defaultUser != null)
            {
                App.SwitchUser(defaultUser);
            }

            var offlineRealmService = (_realmServiceFactory.GetInstance(DataSourceType.Offline) as OfflineRealmService)!;
            offlineRealmService.Initialize();
        }
    }
}
