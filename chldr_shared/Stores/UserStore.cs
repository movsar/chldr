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

        private readonly LocalStorageService _localStorageService;
        private readonly EnvironmentService _environmentService;
        private readonly UserService _userService;
        public ActiveSession ActiveSession { get; private set; } = new ActiveSession();
        #endregion

        public UserStore(UserService userService, EnvironmentService environmentService, NetworkService networkService, LocalStorageService localStorageService)
        {
            // Let people log in if they want (on prod turn off this for Web)
            if (/*environmentService.CurrentPlatform != Platforms.Web &&*/ networkService.IsNetworUp)
            {
                ActiveSession.Status = SessionStatus.Anonymous;
            }

            _localStorageService = localStorageService;
            _environmentService = environmentService;
            _userService = userService;

            RestoreLastSession();
        }

        private async Task SaveActiveSession()
        {
            await _localStorageService.SetItem<ActiveSession>("session", ActiveSession);
        }

        private async Task RestoreLastSession()
        {
            var session = await _localStorageService.GetItem<ActiveSession>("session");
            if (session != null)
            {
                ActiveSession = session;
            }

            // TODO: Validate access token

            UserStateHasChanged?.Invoke();
        }

        public async Task RegisterNewUser(string email, string password)
        {
            await _userService.RegisterNewUserAsync(email, password);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                ActiveSession = await _userService.LogInEmailPasswordAsync(email, password);
                await SaveActiveSession();

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

        public async Task LogOutAsync()
        {
            _userService.LogOutAsync();
            ActiveSession.Clear();
            await SaveActiveSession();
            UserStateHasChanged?.Invoke();
        }
    }
}
