using chldr_dataaccess.Interfaces;
using chldr_dataaccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Stores
{
    internal class UserStore
    {
        private IDataAccess _dataAccess;

        internal UserStore(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        internal async Task LogInEmailPassword(string email, string password)
        {
            await _dataAccess.Login(email, password);
        }
    }
}
