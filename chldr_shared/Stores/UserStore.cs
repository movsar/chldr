using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_shared.Services;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Stores
{
    public class UserStore
    {
        public bool LoggedIn { get; set; } = false;
        public event Action LoggedInUserChanged;
        public UserModel? CurrentUser { get; set; }
        private IDataAccess _dataAccess;

        public UserStore(IDataAccess dataAccess, EnvironmentService environmentService)
        {
            _dataAccess = dataAccess;

            // Don't run anything on load if it's web, otherwise it produces Websocket error
            if (environmentService.CurrentPlatform != Enums.Platforms.Web)
            {
                _dataAccess.DatabaseInitialized += DataAccess_DatabaseInitialized;
            }
        }

        private async void DataAccess_DatabaseInitialized()
        {

            await SetCurrentUserInfoAsync();
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
                var GetCurrentUserInfoTask = new Task(async () => await SetCurrentUserInfoAsync());
                GetCurrentUserInfoTask.Start();
            }
            catch (Exception ex)
            {
                // Localize exception
                throw;
            }
        }

        internal async Task ConfirmUserAsync(string token, string tokenId, string email)
        {
            await _dataAccess.ConfirmUserAsync(token, tokenId, email);
        }

        internal async Task SendPasswordResetRequestAsync(string email)
        {
            await _dataAccess.SendPasswordResetRequestAsync(email);
        }

        internal async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await _dataAccess.UpdatePasswordAsync(token, tokenId, newPassword);
        }

        internal async Task LogOutAsync()
        {
            await _dataAccess.LogOutAsync();
            CurrentUser = null;
            LoggedInUserChanged?.Invoke();
        }
    }
}
