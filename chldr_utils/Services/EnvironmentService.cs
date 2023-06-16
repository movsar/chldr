using chldr_shared.Enums;

namespace chldr_utils.Services
{
    public class EnvironmentService
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
