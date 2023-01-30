using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_utils
{
    public static class AppConstants
    {
        public const string Host = "https://dosham.azurewebsites.net";

      

        public static class DataErrorMessages
        {
            public const string NetworkIsDown = "Network is down";

            public const string AnonymousUser = "Not a registered user";

            public const string AppNotInitialized = "App is not initialized";

            public const string NoUserInfo = "No custom user info is found";
        }
    }
}
