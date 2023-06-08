﻿using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;
using Realms;
using chldr_data.local.RealmEntities;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;

namespace chldr_data.Repositories
{
    public class RealmUsersRepository : RealmRepository<RealmUser, UserModel, UserDto>, IUsersRepository
    {
        public RealmUsersRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        protected override RecordType RecordType => RecordType.User;

        public override async Task Insert(string userId, UserDto dto)
        {
            throw new NotImplementedException();
        }

        public override Task Delete(string userId, string entityId)
        {
            throw new NotImplementedException();
        }

        public override UserModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(string userId, UserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
