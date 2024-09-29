using Bogus;
using domain.DatabaseObjects.Dtos;

namespace chldr_testing_framework.Generators
{
    internal class SourceDtoFaker : Faker<SourceDto>
    {
        internal SourceDtoFaker()
        {
            RuleFor(s => s.SourceId, f => Guid.NewGuid().ToString());
            RuleFor(s => s.UserId, f => f.Random.Guid().ToString());
            RuleFor(s => s.Name, f => f.Name.FullName());
            RuleFor(s => s.CreatedAt, f => f.Date.PastOffset());
            RuleFor(s => s.UpdatedAt, f => f.Date.RecentOffset());
        }
    }
}
