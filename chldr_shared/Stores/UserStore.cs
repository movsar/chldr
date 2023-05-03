using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_utils.Services;

namespace chldr_shared.Stores
{
    public class UserStore
    {
        #region Properties, Events and Fields
        public event Action UserStateHasChanged;

        private readonly EnvironmentService _environmentService;
        private readonly UserService _userService;
        public ActiveSession ActiveSession { get; } = new ActiveSession();
        #endregion

        public UserStore(UserService userService, EnvironmentService environmentService, NetworkService networkService)
        {
            // Let people log in if they want (on prod turn off this for Web)
            if (/*environmentService.CurrentPlatform != Platforms.Web &&*/ networkService.IsNetworUp)
            {
                ActiveSession.Status = SessionStatus.Anonymous;
            }

            _environmentService = environmentService;
            _userService = userService;
        }

        public async Task RegisterNewUser(string email, string password)
        {
            await _userService.RegisterNewUserAsync(email, password);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                var activeSession = await _userService.LogInEmailPasswordAsync(email, password);

                // TODO: Save somewhere refresh and access tokens with expiresIn value
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

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await _userService.UpdatePasswordAsync(token, newPassword);
        }

        public void LogOutAsync()
        {
            _userService.LogOutAsync();
            ActiveSession.Clear();
            UserStateHasChanged?.Invoke();
        }
    }
}
