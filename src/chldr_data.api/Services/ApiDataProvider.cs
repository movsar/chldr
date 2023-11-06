using chldr_data.Interfaces;
using chldr_data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.api.Services
{
    public class ApiDataProvider : IDataProvider
    {
        public ApiDataProvider(RequestService requestService)
        {

        }

        public bool IsInitialized { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action? DatabaseInitialized;

        public IDataAccessor CreateUnitOfWork(string? userId = null)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void TruncateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
