﻿using chldr_api.GraphQL.MutationResolvers;
using chldr_api.GraphQL.MutationServices;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Models;

namespace chldr_api
{
    public class Mutation
    {
        private readonly UserResolver _userResolver;
        private readonly EntryResolver _entryResolver;

        public Mutation(UserResolver userResolver, EntryResolver entryResolver)
        {
            _userResolver = userResolver;
            _entryResolver = entryResolver;
        }

        // Entry mutations
        public async Task<RequestResult> PromoteAsync(string recordTypeName, string userId, string entryId)
        {
            var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);
            switch (recordType)
            {
                case RecordType.Entry:
                    return await _entryResolver.PromoteAsync(userId, entryId);

                case RecordType.Translation:
                    throw new NotImplementedException();

                default:
                    return new RequestResult();
            }
        }

        public async Task<RequestResult> AddEntry(string userId, EntryDto entryDto)
        {
            return await _entryResolver.AddEntryAsync(userId, entryDto);
        }

        public async Task<RequestResult> UpdateEntry(string userId, EntryDto entryDto)
        {
            return await _entryResolver.UpdateEntry(userId, entryDto);
        }

        public async Task<RequestResult> RemoveEntry(string userId, string entryId)
        {
            return await _entryResolver.RemoveEntry(userId, entryId);
        }

        // User mutations
        public async Task<RequestResult> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            return await _userResolver.RegisterAsync(email, password, firstName, lastName, patronymic);
        }

        public async Task<RequestResult> ConfirmEmailAsync(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<RequestResult> PasswordReset(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<RequestResult> UpdatePasswordAsync(string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<RequestResult> LogInRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<RequestResult> LoginEmailPasswordAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }

}