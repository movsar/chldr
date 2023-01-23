using chldr_data.Interfaces;
using chldr_data.Services;
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
        private IDataAccess _dataAccess;

        public UserStore(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            await _dataAccess.LogInEmailPasswordAsync(email, password);
        }

        internal async Task SendPasswordResetRequestAsync(string email)
        {
            await _dataAccess.SendPasswordResetRequestAsync(email);
        }

        internal async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await _dataAccess.UpdatePasswordAsync(token, tokenId, newPassword);
        }
    }
}
