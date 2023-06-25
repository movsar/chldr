using chldr_data.Services;

namespace chldr_utils.Services
{
    public class AuthService
    {
        private readonly RequestService _requestService;

        public AuthService(RequestService requestService)
        {
            _requestService = requestService;
        }
    }
}
