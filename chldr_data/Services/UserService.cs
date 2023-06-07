using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.ResponseTypes;
using chldr_utils;
using chldr_utils.Services;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using Realms.Sync;
using MySqlX.XDevAPI;

namespace chldr_data.Services
{
    public class UserService
    {
        public event Action<ActiveSession>? UserStateHasChanged;
        private readonly NetworkService _networkService;
        private readonly AuthService _authService;
        private readonly IDataProvider _dataProvider;
        private readonly LocalStorageService _localStorageService;
        private ActiveSession _currentSession = new ActiveSession();

        public UserService(NetworkService networkService, IDataProvider dataProvider, AuthService authService, LocalStorageService localStorageService)
        {
            _networkService = networkService;
            _authService = authService;
            _dataProvider = dataProvider;
            _localStorageService = localStorageService;

            _dataProvider.LocalDatabaseInitialized += RealmService_DatasourceInitialized;
        }
        private void RealmService_DatasourceInitialized()
        {
            // If there are things to be done after local database is initialized, do them here
        }

        public async Task<string> RegisterNewUserAsync(string email, string password)
        {
            return await _authService.RegisterUserAsync(email, password);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _authService.PasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await _authService.UpdatePasswordAsync(token, newPassword);
        }

        public async Task ConfirmUserAsync(string token)
        {
            await _authService.ConfirmUserAsync(token);
        }
        private async Task SaveActiveSession()
        {
            await _localStorageService.SetItem<ActiveSession>("session", _currentSession);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                _currentSession = await _authService.LogInEmailPasswordAsync(email, password);
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
            return await _authService.RefreshTokens(refreshToken);
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
