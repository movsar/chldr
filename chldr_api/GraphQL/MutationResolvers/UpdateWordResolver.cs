using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.ResponseTypes;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using chldr_data.Repositories;
using chldr_data.DatabaseObjects.Models;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        internal async Task<UpdateResponse> ExecuteAsync(UnitOfWork unitOfWork, UserDto userDto, WordDto updatedWordDto)
        {
            var existingWord = unitOfWork.Words.Get(updatedWordDto.WordId);
            var existingWordDto = WordDto.FromModel(existingWord);

            var translationChangeSets = UpdateTranslations(unitOfWork, userDto, existingWordDto, updatedWordDto);
            var wordChangeSets = unitOfWork.Words.Update(userDto.UserId, updatedWordDto);
            
            var changesets = translationChangeSets.Union(wordChangeSets);

            // Retrieve back the same changesets so that they'll have indexes
            var updatedChangeSets = unitOfWork.ChangeSets.Get(changesets.Select(c => c.ChangeSetId).ToArray());

            // Return a list of changesets with all the changes made
            var response = new UpdateResponse() { Success = true };
            response.ChangeSets.AddRange(updatedChangeSets.Select(c => ChangeSetDto.FromModel(c)));
            return response;
        }

        private List<ChangeSetModel> UpdateTranslations(UnitOfWork unitOfWork, UserDto user, WordDto existingWordDto, WordDto updatedWordDto)
        {
            // Create a changeset with all the differences between existing and updated objects

            // ! There should be a separate ChangeSet for each changed / inserted / deleted object
            var existingTranslationIds = existingWordDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedWordDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedWordDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingWordDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedWordDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            var changeSets = new List<ChangeSetModel>();
            foreach (var insertedTranslation in insertedTranslations)
            {
                changeSets.AddRange(unitOfWork.Translations.Add(user.UserId, insertedTranslation));
            }

            foreach (var deletedTranslation in deletedTranslations)
            {
                changeSets.AddRange(unitOfWork.Translations.Delete(user.UserId, deletedTranslation.TranslationId));
            }

            foreach (var translationDto in updatedTranslations)
            {
                changeSets.AddRange(unitOfWork.Translations.Update(user.UserId, translationDto));
            }

            return changeSets;
        }
    }
}