using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_shared.Enums;
using chldr_shared.Services;
using Microsoft.Extensions.Logging;

namespace chldr_shared.Stores
{
    public class UserStore
    {

        #region Properties
        public UserModel? CurrentUserInfo { get; set; }
        public SessionStatus SessionStatus
        {
            get => _sessionStatus;
            set
            {
                _sessionStatus = value;
                UserStateHasChanged?.Invoke();
            }
        }
        #endregion

        #region Events
        public event Action UserStateHasChanged;
        #endregion

        #region Fields
        private readonly ILogger<UserStore> _logger;
        private readonly IDataAccess _dataAccess;
        private readonly EnvironmentService _environmentService;
        private SessionStatus _sessionStatus;
        #endregion

        #region Constructors
        public UserStore(IDataAccess dataAccess, EnvironmentService environmentService, ILogger<UserStore> logger)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _environmentService = environmentService;

            _dataAccess.DatabaseSynchronized += DataAccess_ChangesSynchronized;
            _dataAccess.DatabaseInitialized += DataAccess_DatabaseInitialized;
            _dataAccess.ConnectionInitialized += DataAccess_ConnectionInitialized;
        }

        private SessionStatus GetSessionStatus(Realms.Sync.User? user)
        {
            if (user == null || user.Id == null || CurrentUserInfo == null)
            {
                return SessionStatus.Disconnected;
            }

            if (_dataAccess.App.CurrentUser.Provider == Realms.Sync.Credentials.AuthProvider.Anonymous)
            {
                return SessionStatus.Unauthorized;
            }

            return SessionStatus.LoggedIn;
        }
        private void DataAccess_ConnectionInitialized()
        {
            UpdateSessionStatus();
        }

        private void DataAccess_ChangesSynchronized()
        {
            UpdateSessionStatus();
        }

        private void DataAccess_DatabaseInitialized()
        {
            try
            {
                if (CurrentUserInfo == null)
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


        public async Task RegisterNewUser(string email, string password)
        {
            await _dataAccess.RegisterNewUserAsync(email, password);
        }

        public async Task SetCurrentUserInfoAsync()
        {
            try
            {
                CurrentUserInfo = await _dataAccess.GetCurrentUserInfoAsync();
                UpdateSessionStatus();
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
                await _dataAccess.Initialize();
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
            UpdateSessionStatus();
        }

        private void UpdateSessionStatus()
        {
            SessionStatus = GetSessionStatus(_dataAccess.App.CurrentUser);
        }
    }
}
