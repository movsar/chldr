using chldr_application.ApiDataProvider.Interfaces;
using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Models;
using core.Interfaces;
using core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_application.ApiDataProvider.Repositories
{
    internal class ApiUsersRepository : ApiRepository<UserModel, UserDto>, IUsersRepository
    {
        public ApiUsersRepository(IRequestService requestService) : base(requestService)
        {
        }

        public override Task Add(UserDto dto)
        {
            throw new NotImplementedException();
        }

        public override Task<UserModel> GetAsync(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<UserModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<UserModel>> TakeAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Update(UserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
