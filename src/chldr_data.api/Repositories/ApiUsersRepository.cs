using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;
using Realms;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Models;
using chldr_data.Services;

namespace chldr_data.Repositories
{
    public class ApiUsersRepository : ApiRepository<UserModel, UserDto>, IUsersRepository
    {
        public ApiUsersRepository(ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }

        protected override RecordType RecordType => RecordType.User;
     
        public override async Task<List<ChangeSetModel>> Add(UserDto dto)
        {
          

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public override async Task<List<ChangeSetModel>> Update(UserDto dto)
        {
          
            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
