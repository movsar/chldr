using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface ITranslationsRepository : IRepository<TranslationModel, TranslationDto>
    {
        public static List<EntryModel> ApplyTranslationFilters(List<EntryModel> entryModels, TranslationFilters filters)
        {
            // Prepare language codes to look for
            var languageCodes = new List<string>();
            if (filters.LanguageCodes != null)
            {
                languageCodes = filters.LanguageCodes.Select(lc => lc.ToLower().Trim()).ToList();
            }

            foreach (var entry in entryModels)
            {
                var translationsToFilterOut = new List<string>();

                foreach (var translation in entry.Translations)
                {
                    // If language codes are specified - filter by them
                    if (languageCodes.Any() && !languageCodes.Contains(translation.LanguageCode.ToLower()))
                    {
                        translationsToFilterOut.Add(translation.TranslationId);
                    }

                    // Filter by rate
                    if (filters.IncludeOnModeration == false && translation.Rate <= UserModel.MemberRateRange.Upper)
                    {
                        translationsToFilterOut.Add(translation.TranslationId);
                    }
                }

                entry.Translations.RemoveAll(t => translationsToFilterOut.Contains(t.TranslationId));
            }

            return entryModels.Where(e => e.Translations.Any()).ToList();
        }
        Task<ChangeSetModel> Promote(ITranslation translation);
    }
}
