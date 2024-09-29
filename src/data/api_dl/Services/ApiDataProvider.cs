using domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_dl.ApiDataProvider.Services
{
    internal class ApiDataProvider : IDataProvider
    {
        private readonly IRequestService _requestService;

        public ApiDataProvider(IRequestService requestService)
        {
            _requestService = requestService;
        }
        public bool IsInitialized { get; set; } = true;

        public event Action? DatabaseInitialized;

        public void Initialize()
        {
            DatabaseInitialized?.Invoke();
        }

        public IDataAccessor Repositories(string? userId = null)
        {
            return new ApiDataAccessor(_requestService);
        }

        public void TruncateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
