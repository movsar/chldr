using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_utils.Services;
using Newtonsoft.Json;

namespace chldr_data.Services
{
    public class UserService
    {
        public event Action<ActiveSession>? UserStateHasChanged;

        private readonly RequestService _requestService;
        private readonly IDataProvider _dataProvider;
        private readonly LocalStorageService _localStorageService;
        private ActiveSession _currentSession = new ActiveSession();

        public UserService(IDataProvider dataProvider, RequestService requestService, LocalStorageService localStorageService)
        {
            _requestService = requestService;
            _dataProvider = dataProvider;
            _localStorageService = localStorageService;

            _dataProvider.DatabaseInitialized += RealmService_DatasourceInitialized;
        }
        private void RealmService_DatasourceInitialized()
        {
            // If there are things to be done after local database is initialized, do them here
        }

        public async Task<string> RegisterNewUserAsync(string email, string password)
        {
            var response = await _requestService.RegisterUserAsync(email, password);

            var token = RequestResult.GetData<string>(response);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("Token shouldn't be null");
            }

            return token;
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _requestService.PasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await _requestService.UpdatePasswordAsync(token, newPassword);
        }

        public async Task ConfirmUserAsync(string token)
        {
            await _requestService.ConfirmUserAsync(token);
        }
        private async Task SaveActiveSession()
        {
            await _localStorageService.SetItem<ActiveSession>("session", _currentSession);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                var response = await _requestService.LogInEmailPasswordAsync(email, password);
                if (!response.Success)
                {
                    throw new Exception("Error:Bad_request");
                }

                var data = RequestResult.GetData<dynamic>(response);

                _currentSession = new ActiveSession()
                {
                    AccessToken = data.AccessToken!,
                    RefreshToken = data.RefreshToken!,
                    AccessTokenExpiresIn = data.AccessTokenExpiresIn!,
                    Status = SessionStatus.LoggedIn,
                    User = data.User
                };

                await SaveActiveSession();

                UserStateHasChanged?.Invoke(_currentSession);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task LogOutAsync()
        {
            _currentSession.Clear();
            await SaveActiveSession();

            UserStateHasChanged?.Invoke(_currentSession);
        }

        public async Task<ActiveSession> RefreshTokens(string refreshToken)
        {
            var response = await _requestService.RefreshTokens(refreshToken);

            if (!response.Success)
            {
                throw new Exception("Error:Bad_request");
            }

            var data = RequestResult.GetData<dynamic>(response);

            return new ActiveSession()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                AccessTokenExpiresIn = data.AccessTokenExpiresIn!,
                Status = SessionStatus.LoggedIn,
                User = data.User
            };
        }

        public async Task RestoreLastSession()
        {
            // Get last session info from the local storage
            var session = await _localStorageService.GetItem<ActiveSession>("session");
            if (session != null)
            {
                _currentSession = session;
                UserStateHasChanged?.Invoke(_currentSession);
            }

            var expired = DateTimeOffset.UtcNow > _currentSession.AccessTokenExpiresIn;
            if (expired && !string.IsNullOrWhiteSpace(_currentSession.RefreshToken))
            {
                // Try to refresh Access Token
                _currentSession = await RefreshTokens(_currentSession.RefreshToken);
                await SaveActiveSession();
                UserStateHasChanged?.Invoke(_currentSession);
            }
        }

    }
}
