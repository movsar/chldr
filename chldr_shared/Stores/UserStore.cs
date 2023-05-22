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
        public event Action UserStateHasChanged;

        private readonly LocalStorageService _localStorageService;
        private readonly EnvironmentService _environmentService;
        private readonly UserService _userService;
        private readonly ExceptionHandler _exceptionHandler;

        public ActiveSession ActiveSession { get; private set; } = new ActiveSession();
        #endregion

        public UserStore(UserService userService, ExceptionHandler exceptionHandler, EnvironmentService environmentService, NetworkService networkService, LocalStorageService localStorageService)
        {
            // Let people log in if they want (on prod turn off this for Web)
            if (/*environmentService.CurrentPlatform != Platforms.Web &&*/ networkService.IsNetworUp)
            {
                ActiveSession.Status = SessionStatus.Anonymous;
            }

            _localStorageService = localStorageService;
            _environmentService = environmentService;
            _userService = userService;
            _exceptionHandler = exceptionHandler;

            Task.Run(async () =>
            {
                try
                {
                    await RestoreLastSession();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.LogAndThrow(ex);
                };
            });
        }

        private async Task SaveActiveSession()
        {
            await _localStorageService.SetItem<ActiveSession>("session", ActiveSession);
        }

        private async Task RestoreLastSession()
        {
            // Get last session info from the local storage
            var session = await _localStorageService.GetItem<ActiveSession>("session");
            if (session != null)
            {
                ActiveSession = session;
                UserStateHasChanged?.Invoke();
            }

            var expired = DateTimeOffset.UtcNow > ActiveSession.AccessTokenExpiresIn;
            if (expired && !string.IsNullOrWhiteSpace(ActiveSession.RefreshToken))
            {
                // Try to refresh Access Token
                ActiveSession = await _userService.RefreshTokens(ActiveSession.RefreshToken);
                await SaveActiveSession();
                UserStateHasChanged?.Invoke();
            }

        }

        public async Task<string> RegisterNewUser(string email, string password)
        {
            return await _userService.RegisterNewUserAsync(email, password);
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
            _userService.LogOutAsync();
            ActiveSession.Clear();
            await SaveActiveSession();
            UserStateHasChanged?.Invoke();
        }
    }
}
