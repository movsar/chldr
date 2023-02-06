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
            userService.UserStateHasChanged += UserService_UserStateHasChanged;
        }

        private void UserService_UserStateHasChanged(UserModel user, SessionStatus sessionStatus)
        {
            CurrentUserInfo = user;
            SessionStatus = sessionStatus;
        }

        public async Task RegisterNewUser(string email, string password)
        {
            await _userService.RegisterNewUserAsync(email, password);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                SessionStatus = SessionStatus.LoggingIn;
                await _userService.LogInEmailPasswordAsync(email, password);
                UserStateHasChanged?.Invoke();
            }
            catch (Exception)
            {
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
