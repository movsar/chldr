using domain;
using domain.Enums;
using domain.Interfaces;

namespace domain.Services
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
