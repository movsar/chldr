using chldr_data.Interfaces;
using dosham.Enums;

namespace dosham.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public EnvironmentService(Platforms platform, bool isDevelopment)
        {
            CurrentPlatform = platform;
            IsDevelopment = isDevelopment;
        }
        private NetworkService _networkService;
        public bool IsDevelopment { get; private set; }
        public Platforms CurrentPlatform { get; set; }


        public bool IsNetworkUp()
        {
            if (_networkService == null)
            {
                _networkService = new NetworkService();
            }
            return _networkService.IsNetworUp;
        }

    }
}
