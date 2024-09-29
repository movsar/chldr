using Bogus;
using domain.DatabaseObjects.Dtos;

namespace chldr_testing_framework.Generators
{
    internal class EntryDtoFaker : Faker<EntryDto>
    {
        internal EntryDtoFaker()
        {
            RuleFor(e => e.EntryId, f => Guid.NewGuid().ToString());
            RuleFor(e => e.UserId, f => Guid.NewGuid().ToString());
            RuleFor(e => e.SourceId, f => Guid.NewGuid().ToString());
            RuleFor(e => e.Rate, f => f.Random.Int(1, 5));
            RuleFor(e => e.Content, f => f.Lorem.Paragraph());
            RuleFor(e => e.Type, f => f.Random.Int(1, 3));
            RuleFor(e => e.Subtype, (f, e) => e.Type == 1 ? f.Random.Int(1, 10) : 0);
            RuleFor(e => e.CreatedAt, f => f.Date.PastOffset());
            RuleFor(e => e.UpdatedAt, f => f.Date.RecentOffset());
        }
    }
}
