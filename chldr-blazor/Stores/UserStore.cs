using Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_native.Stores
{
    internal class UserStore
    {
        private DataAccess _dataAccess;

        internal UserStore(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        internal async Task LogInEmailPassword(string email, string password)
        {
            await _dataAccess.Login(email, password);
        }
    }
}
