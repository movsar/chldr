using Bogus;
using sql_dl.SqlEntities;

namespace chldr_test_utils.Generators
{
    internal class UserFaker : Faker<SqlUser>
    {
        public UserFaker()
        {
            RuleFor(u => u.Id, f => Guid.NewGuid().ToString());
            RuleFor(u => u.Email, f => f.Person.Email);
            RuleFor(u => u.Rate, f => f.Random.Int(1, 5));
            RuleFor(u => u.Patronymic, f => f.Name.FirstName());
            RuleFor(u => u.FirstName, f => f.Name.FirstName());
            RuleFor(u => u.LastName, f => f.Name.LastName());
            RuleFor(u => u.ImagePath, f => f.Internet.Url());
            RuleFor(u => u.CreatedAt, f => f.Date.PastOffset());
            RuleFor(u => u.UpdatedAt, f => f.Date.RecentOffset());
        }
    }
}
