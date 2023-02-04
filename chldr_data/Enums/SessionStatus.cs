using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Enums
{
    public enum SessionStatus
    {
        // Offline
        Disconnected = 0,

        // Anonumous user logged in
        Unauthorized = 1,

        LoggingIn = 2,
        LoggedIn = 3,
    }
}
