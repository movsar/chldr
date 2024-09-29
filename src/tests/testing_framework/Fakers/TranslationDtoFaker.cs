using Bogus;
using domain.DatabaseObjects.Dtos;

namespace chldr_testing_framework.Generators
{
    internal class TranslationDtoFaker : Faker<TranslationDto>
    {
        private static readonly Random random = new Random();
        public static string SelectRandomLanguageCode()
        {
            string[] languageCodes = { "RUS", "ENG", "CHE", "LAT" };
            int index = random.Next(languageCodes.Length);
            return languageCodes[index];
        }
        internal TranslationDtoFaker()
        {
            RuleFor(t => t.UserId, f => f.Random.Guid().ToString());
            RuleFor(t => t.EntryId, f => f.Random.Guid().ToString());
            RuleFor(t => t.LanguageCode, f => SelectRandomLanguageCode());
            RuleFor(t => t.Content, f => f.Lorem.Sentence());
            RuleFor(t => t.Rate, f => f.Random.Int(1, 5));
            RuleFor(t => t.TranslationId, f => Guid.NewGuid().ToString());
            RuleFor(t => t.Notes, f => f.Random.Bool(3) ? f.Lorem.Sentence() : null);
            RuleFor(t => t.CreatedAt, f => f.Date.PastOffset());
            RuleFor(t => t.UpdatedAt, f => f.Date.RecentOffset());
        }
    }
}
