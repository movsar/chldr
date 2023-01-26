using chldr_shared.Enums;

namespace chldr_shared.Services
{
    public class EnvironmentService
    {
        public Platforms CurrentPlatform { get; set; }
        public EnvironmentService(Platforms platform)
        {
            CurrentPlatform = platform;
        }
    }
}
