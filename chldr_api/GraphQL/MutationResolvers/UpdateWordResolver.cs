using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Interfaces;
using chldr_data.ResponseTypes;
using chldr_data.Services;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        internal async Task<MutationResponse> ExecuteAsync(IUnitOfWork unitOfWork, string userId, WordDto updatedWordDto)
        {
            await unitOfWork.Words.Update(userId, updatedWordDto);

            return new MutationResponse() { Success = true };
        }     
    }
}