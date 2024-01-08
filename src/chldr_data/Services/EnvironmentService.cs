using chldr_data.Enums;
using chldr_data.Interfaces;

namespace chldr_data.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public EnvironmentService(Platforms platform, bool isDevelopment)
        {
            CurrentPlatform = platform;
            IsDevelopment = isDevelopment;
        }
        public bool IsDevelopment { get; private set; }
        public Platforms CurrentPlatform { get; set; }
    }
}
