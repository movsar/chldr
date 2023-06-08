using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.ResponseTypes;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Services;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        internal async Task<MutationResponse> ExecuteAsync(SqlUnitOfWork unitOfWork, string userId, WordDto updatedWordDto)
        {
            var existingWord = unitOfWork.Words.Get(updatedWordDto.WordId);
            var existingWordDto = WordDto.FromModel(existingWord);

            await UpdateTranslations(unitOfWork, userId, existingWordDto, updatedWordDto);
            await unitOfWork.Words.Update(userId, updatedWordDto);

            // Retrieve back the same changesets so that they'll have indexes

            // Return a list of changesets with all the changes made
            return new MutationResponse() { Success = true };
        }

        private async Task UpdateTranslations(SqlUnitOfWork unitOfWork, string userId, WordDto existingWordDto, WordDto updatedWordDto)
        {
            // Create a changeset with all the differences between existing and updated objects

            // ! There should be a separate ChangeSet for each changed / inserted / deleted object
            var existingTranslationIds = existingWordDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedWordDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedWordDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingWordDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedWordDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var insertedTranslation in insertedTranslations)
            {
                await unitOfWork.Translations.Add(userId, insertedTranslation);
            }

            foreach (var deletedTranslation in deletedTranslations)
            {
                await unitOfWork.Translations.Delete(userId, deletedTranslation.TranslationId);
            }

            foreach (var translationDto in updatedTranslations)
            {
                await unitOfWork.Translations.Update(userId, translationDto);
            }
        }
    }
}