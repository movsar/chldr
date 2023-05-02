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

            _realmService = (realmServiceFactory.GetInstance(DataSourceType.Synced) as SyncedRealmService)!;
            _realmService.DatasourceInitialized += RealmService_DatasourceInitialized;
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
            var appUserId = App.CurrentUser.Id;
            var user = Database.All<Entities.RealmUser>().FirstOrDefault(u => u.UserId == appUserId);

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
            var graphQlClient = new GraphQLHttpClient("https://localhost:7065/graphql/", new NewtonsoftJsonSerializer());
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($email: String!) {
                            initiatePasswordReset(email: $email) {
                                success
                                errorMessage
                                resetToken
                            }
                        }",

                Variables = new { email }
            };

            var response = await graphQlClient.SendMutationAsync<JObject>(request);
            var responseData = response.Data["initiatePasswordReset"].ToObject<InitiatePasswordResetResponse>();

            if (responseData.Success)
            {
                // Password reset initiated successfully
                var resetToken = responseData.ResetToken;
            }
            else
            {
                // Password reset initiation failed
                var errorMessage = responseData.ErrorMessage;
            }
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            //await App.EmailPasswordAuth.ResetPasswordAsync(newPassword, token);
        }

        public async Task ConfirmUserAsync(string token, string tokenId, string userEmail)
        {
            await App.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            await App.LogInAsync(Credentials.EmailPassword(email, password));
            _realmService.Initialize();
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
