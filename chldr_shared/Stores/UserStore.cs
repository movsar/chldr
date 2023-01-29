using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_shared.Services;
using Microsoft.Extensions.Logging;

namespace chldr_shared.Stores
{
    public class UserStore
    {

        #region Properties
        public bool LoggedIn { get; set; } = false;
        public UserModel? CurrentUser { get; set; }
        #endregion

        #region Events
        public event Action LoggedInUserChanged;
        #endregion

        #region Fields
        private readonly ILogger<UserStore> _logger;
        private readonly IDataAccess _dataAccess;
        private readonly EnvironmentService _environmentService;
        #endregion

        #region Constructors
        public UserStore(IDataAccess dataAccess, EnvironmentService environmentService, ILogger<UserStore> logger)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _environmentService = environmentService;

            _dataAccess.DatabaseInitialized += DataAccess_DatabaseInitialized;
            _dataAccess.DatabaseSynced += DataAccess_DatabaseSynced;
        }

        private void DataAccess_DatabaseSynced()
        {
            try
            {
                if (CurrentUser == null)
                {
                    // All the other ways of starting this async job result in an WebSocket's error when deployed
                    new Task(async () => await SetCurrentUserInfoAsync()).Start();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        #endregion

        #region Event Handlers
        private void DataAccess_DatabaseInitialized()
        {

        }
        #endregion

        public async Task RegisterNewUser(string email, string password)
        {
            await _dataAccess.RegisterNewUserAsync(email, password);
        }

        public async Task SetCurrentUserInfoAsync()
        {
            try
            {
                CurrentUser = await _dataAccess.GetCurrentUserInfoAsync();
                LoggedInUserChanged?.Invoke();
            }
            catch (Exception ex)
            {
                // Localize exception
                throw;
            }
        }
        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            try
            {
                await _dataAccess.LogInEmailPasswordAsync(email, password);
                LoggedIn = true;

                new Task(async () => await SetCurrentUserInfoAsync()).Start();
            }
            catch (Exception ex)
            {
                // Localize exception
                throw;
            }
        }

        public async Task ConfirmUserAsync(string token, string tokenId, string email)
        {
            await _dataAccess.ConfirmUserAsync(token, tokenId, email);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _dataAccess.SendPasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await _dataAccess.UpdatePasswordAsync(token, tokenId, newPassword);
        }

        public async Task LogOutAsync()
        {
            await _dataAccess.LogOutAsync();
            CurrentUser = null;
            LoggedInUserChanged?.Invoke();
        }
    }
}
