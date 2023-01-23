using chldr_shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Services
{
    public static class EnvironmentService
    {
        public static Platforms CurrentPlatform
        {
            get
            {
#if ANDROID
                return Platforms.Android;
#elif IOS
                return Platforms.IOS;
#elif WINDOWS
                return Platforms.Windows;
#else
                return Platforms.Web;
#endif
            }
        }
    }
}
