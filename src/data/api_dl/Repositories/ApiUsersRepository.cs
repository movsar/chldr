using api_dl.ApiDataProvider.Interfaces;
using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;
using domain.Interfaces;
using domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_dl.ApiDataProvider.Repositories
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
