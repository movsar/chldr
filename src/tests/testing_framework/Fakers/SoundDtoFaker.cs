using Bogus;
using domain.DatabaseObjects.Dtos;

namespace chldr_testing_framework.Generators
{
    internal class SoundDtoFaker : Faker<SoundDto>
    {
        internal SoundDtoFaker()
        {
            RuleFor(s => s.SoundId, f => Guid.NewGuid().ToString());
            RuleFor(s => s.FileName, f => Guid.NewGuid().ToString());
            RuleFor(s => s.EntryId, f => f.Random.Guid().ToString());
            RuleFor(s => s.UserId, f => f.Random.Guid().ToString());
            RuleFor(s => s.RecordingB64, f => f.Random.Double() < 0.7 ? Convert.ToBase64String(Guid.NewGuid().ToByteArray()) : null);
            RuleFor(s => s.CreatedAt, f => f.Date.PastOffset());
            RuleFor(s => s.UpdatedAt, f => f.Date.RecentOffset());
        }
    }
}
