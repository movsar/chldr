using chldr_shared.Enums;
using System.Security.Cryptography.X509Certificates;

namespace chldr_utils.Services
{
    public class EnvironmentService
    {
        public bool IsDevelopment { get; private set; }
        public Platforms CurrentPlatform { get; set; }
        public EnvironmentService(Platforms platform, bool isDevelopment)
        {
            CurrentPlatform = platform;
            IsDevelopment = isDevelopment;
        }
    }
}
