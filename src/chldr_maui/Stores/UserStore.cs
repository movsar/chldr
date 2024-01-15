using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using dosham.Services;
using chldr_utils;
using chldr_utils.Services;

namespace dosham.Stores
{
    public class UserStore
    {
        #region Properties, Events and Fields

        private readonly UserService _userService;
        private readonly ExceptionHandler _exceptionHandler;
        public event Action UserStateHasChanged;
        public UserModel? CurrentUser { get; set; } = null;
        public bool IsLoggedIn => CurrentUser != null;
        #endregion

        public UserStore(UserService userService, ExceptionHandler exceptionHandler)
        {
            // Let people log in if they want (on prod turn off this for Web)
            _exceptionHandler = exceptionHandler;
            _userService = userService;
            _userService.UserStateHasChanged += UserStore_UserStateHasChanged;
        }

        public async Task RestoreLastSession()
        {
            try
            {
                await _userService.RestoreLastSession();
            }
            catch (Exception ex)
            {
                throw _exceptionHandler.Error(ex);
            };
        }

        private void UserStore_UserStateHasChanged(SessionInformation activeSession)
        {
            CurrentUser = _userService.CurrentSession?.UserDto == null ? null : UserModel.FromDto(_userService.CurrentSession.UserDto);
            UserStateHasChanged?.Invoke();
        }

        public async Task RegisterNewUser(string email, string password)
        {
            await _userService.RegisterNewUserAsync(email, password);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            await _userService.LogInEmailPasswordAsync(email, password);
        }

        public async Task ConfirmUserAsync(string token)
        {
            await _userService.ConfirmUserAsync(token);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _userService.SendPasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string email, string token, string newPassword)
        {
            await _userService.UpdatePasswordAsync(email, token, newPassword);
        }

        public async Task LogOutAsync()
        {
            await _userService.LogOutAsync();
        }
    }
}
