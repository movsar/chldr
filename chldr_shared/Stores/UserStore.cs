using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_shared.Stores
{
    public class UserStore
    {
        #region Properties, Events and Fields

        private readonly UserService _userService;
        private readonly ExceptionHandler _exceptionHandler;

        public ActiveSession ActiveSession { get; private set; } = new ActiveSession();
        public Action UserStateHasChanged { get; set; }
        #endregion

        public UserStore(UserService userService, ExceptionHandler exceptionHandler)
        {
            // Let people log in if they want (on prod turn off this for Web)
            _exceptionHandler = exceptionHandler;
            _userService = userService;
            _userService.UserStateHasChanged += UserStore_UserStateHasChanged;

            Task.Run(async () =>
            {
                try
                {
                    await _userService.RestoreLastSession();
                }
                catch (Exception ex)
                {
                    throw _exceptionHandler.Error(ex);
                };
            });
        }

        private void UserStore_UserStateHasChanged(ActiveSession activeSession)
        {
            ActiveSession = activeSession;
            UserStateHasChanged?.Invoke();
        }

        public async Task<string> RegisterNewUser(string email, string password)
        {
            return await _userService.RegisterNewUserAsync(email, password);
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

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await _userService.UpdatePasswordAsync(token, newPassword);
        }

        public async Task LogOutAsync()
        {
            await _userService.LogOutAsync();
        }
    }
}
