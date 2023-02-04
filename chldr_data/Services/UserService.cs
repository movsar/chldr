using chldr_data.Enums;
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
        private readonly NetworkService _networkService;
        private readonly SyncedRealmService _realmService;
    
        App App => _realmService.GetApp();
        Realm Database => _realmService.GetDatabase();
        public UserService(NetworkService networkService, IRealmServiceFactory realmServiceFactory)
        {
            _realmService = realmServiceFactory.GetInstance(DataAccessType.Synced) as SyncedRealmService;
            _networkService = networkService;

          
        }
       
        public UserModel GetCurrentUserInfo()
        {
            if (!_networkService.IsNetworUp || Database.SyncSession.State == SessionState.Inactive)
            {
                throw new Exception(AppConstants.DataErrorMessages.NetworkIsDown);
            }

            if (App?.CurrentUser?.Id == null)
            {
                throw new Exception(AppConstants.DataErrorMessages.AppNotInitialized);
            }

            if (App.CurrentUser.Provider == Credentials.AuthProvider.Anonymous)
            {
                throw new Exception(AppConstants.DataErrorMessages.AnonymousUser);
            }

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
            // Don't touch this unless it's absolutely necessary! It was very hard to configure!
            var appUser = await App.LogInAsync(Credentials.EmailPassword(email, password));
            _realmService.InitializeConfiguration();
        }

        public async Task LogOutAsync()
        {
            var anonymousUser = App.AllUsers.FirstOrDefault(u => u.Provider == Credentials.AuthProvider.Anonymous);
            if (anonymousUser == null)
            {
                throw new Exception("not good");
            }

            App.SwitchUser(anonymousUser);
            _realmService.InitializeConfiguration();
        }
    }
}
