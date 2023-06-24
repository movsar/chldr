using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_data.Services;
using chldr_utils.Interfaces;
using GraphQL;

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
