using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_shared.Enums;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_shared.Stores
{
    public class UserStore
    {
        #region Properties, Events and Fields
        public event Action UserStateHasChanged;

        private readonly EnvironmentService _environmentService;
        private readonly UserService _userService;
        private SessionStatus _sessionStatus = SessionStatus.Disconnected;
        public SessionStatus SessionStatus
        {
            get => _sessionStatus;
            set
            {
                _sessionStatus = value;
                UserStateHasChanged?.Invoke();
            }
        }
        public UserModel? CurrentUserInfo { get; set; }
      
        #endregion

        public UserStore(UserService userService, EnvironmentService environmentService, NetworkService networkService)
        {
            // Let people log in if they want (on prod turn off this for Web)
            if (/*environmentService.CurrentPlatform != Platforms.Web &&*/ networkService.IsNetworUp)
            {
                SessionStatus = SessionStatus.Unauthorized;
            }

            _environmentService = environmentService;
            _userService = userService;
            //_dataAccess.DatabaseInitialized += DataAccess_DatabaseInitialized;
            //_dataAccess.DatabaseSynchronized += DataAccess_ChangesSynchronized;

        }

        //private void DataAccess_ChangesSynchronized()
        //{
        //    // This is going to refresh logged in user info, when you register, the database is not ready, 
        //    // the status is LoggingIn but need to wait for the custom user data
        //    if (CurrentUserInfo == null)
        //    {
        //        new Task(() => SetCurrentUserInfo()).Start();
        //    }
        //}

        //private void DataAccess_DatabaseInitialized()
        //{
        //    if (CurrentUserInfo != null)
        //    {
        //        SessionStatus = SessionStatus.LoggedIn;
        //    }
        //    else
        //    {
        //        // Request the custom user data on database initialized
        //        new Task(() => SetCurrentUserInfo()).Start();
        //    }
        //}
        public void SetCurrentUserInfo()
        {
            try
            {
                CurrentUserInfo = _userService.GetCurrentUserInfo();
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

        public async Task RegisterNewUser(string email, string password)
        {
            await _userService.RegisterNewUserAsync(email, password);
        }

     
        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                await _userService.LogInEmailPasswordAsync(email, password);
            }
            catch (Exception ex)
            {
                // Localize exception
                throw;
            }
        }

        public async Task ConfirmUserAsync(string token, string tokenId, string email)
        {
            await _userService.ConfirmUserAsync(token, tokenId, email);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _userService.SendPasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await _userService.UpdatePasswordAsync(token, tokenId, newPassword);
        }

        public async Task LogOutAsync()
        {
            await _userService.LogOutAsync();
            CurrentUserInfo = null;
            SessionStatus = SessionStatus.Unauthorized;
            UserStateHasChanged?.Invoke();
        }
    }
}
