using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_shared.Enums;
using chldr_shared.Services;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.Extensions.Logging;

namespace chldr_shared.Stores
{
    public class UserStore
    {
        #region Properties, Events and Fields
        public event Action UserStateHasChanged;

        private readonly ISyncedDataAccess _dataAccess;
        private readonly EnvironmentService _environmentService;
        public UserModel? CurrentUserInfo { get; set; }
        private SessionStatus _sessionStatus;
        public SessionStatus SessionStatus
        {
            get => _sessionStatus;
            set
            {
                _sessionStatus = value;
                UserStateHasChanged?.Invoke();
            }
        }
        #endregion

        public UserStore(IDataAccessFactory dataAccessFactory, EnvironmentService environmentService)
        {
            _dataAccess = dataAccessFactory.GetInstance(DataAccessType.Synced) as SyncedDataAccess;

            _environmentService = environmentService;

            _dataAccess.DatabaseSynchronized += DataAccess_ChangesSynchronized;
            _dataAccess.DatabaseInitialized += DataAccess_DatabaseInitialized;
        }

        private void DataAccess_ChangesSynchronized()
        {
            // This is going to refresh logged in user info, when you register, the database is not ready, 
            // the status is LoggingIn but need to wait for the custom user data
            if (CurrentUserInfo == null)
            {
                new Task(() => SetCurrentUserInfo()).Start();
            }
        }

        private void DataAccess_DatabaseInitialized()
        {
            if (CurrentUserInfo != null)
            {
                SessionStatus = SessionStatus.LoggedIn;
            }
            else
            {
                // Request the custom user data on database initialized
                new Task(() => SetCurrentUserInfo()).Start();
            }
        }

        public async Task RegisterNewUser(string email, string password)
        {
            await _dataAccess.UserService.RegisterNewUserAsync(email, password);
        }

        public void SetCurrentUserInfo()
        {
            try
            {
                CurrentUserInfo = _dataAccess.UserService.GetCurrentUserInfo();
                SessionStatus = SessionStatus.LoggedIn;
                UserStateHasChanged?.Invoke();
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case AppConstants.DataErrorMessages.NetworkIsDown:
                        SessionStatus = SessionStatus.Disconnected;
                        break;

                    case AppConstants.DataErrorMessages.AnonymousUser:
                        SessionStatus = SessionStatus.Unauthorized;
                        break;

                    case AppConstants.DataErrorMessages.AppNotInitialized:
                        SessionStatus = SessionStatus.Disconnected;
                        break;

                    case AppConstants.DataErrorMessages.NoUserInfo:
                        SessionStatus = SessionStatus.LoggingIn;
                        break;

                    default:
                        break;
                }
            }
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                await _dataAccess.UserService.LogInEmailPasswordAsync(email, password);
                _dataAccess.Initialize();
            }
            catch (Exception ex)
            {
                // Localize exception
                throw;
            }
        }

        public async Task ConfirmUserAsync(string token, string tokenId, string email)
        {
            await _dataAccess.UserService.ConfirmUserAsync(token, tokenId, email);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _dataAccess.UserService.SendPasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await _dataAccess.UserService.UpdatePasswordAsync(token, tokenId, newPassword);
        }

        public async Task LogOutAsync()
        {
            await _dataAccess.UserService.LogOutAsync();
            CurrentUserInfo = null;
            SessionStatus = SessionStatus.Unauthorized;
            UserStateHasChanged?.Invoke();
        }
    }
}
