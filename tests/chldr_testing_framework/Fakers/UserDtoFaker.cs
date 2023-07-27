﻿using Bogus;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;

namespace chldr_test_utils.Generators
{
    internal class UserDtoFaker : Faker<UserDto>
    {
        public UserDtoFaker()
        {
            RuleFor(u => u.UserId, f => Guid.NewGuid().ToString());
            RuleFor(u => u.Email, f => f.Person.Email);
            RuleFor(u => u.Password, f => f.Internet.Password());
            RuleFor(u => u.PasswordConfirmation, (f, u) => u.Password);
            RuleFor(u => u.RateWeight, f => f.Random.Int(1, 100));
            RuleFor(u => u.Rate, f => f.Random.Int(1, 5));
            RuleFor(u => u.Patronymic, f => f.Name.FirstName());
            RuleFor(u => u.FirstName, f => f.Name.FirstName());
            RuleFor(u => u.LastName, f => f.Name.LastName());
            RuleFor(u => u.Status, f => UserStatus.Active);
            RuleFor(u => u.ImagePath, f => f.Internet.Url());
            RuleFor(u => u.CreatedAt, f => f.Date.PastOffset());
            RuleFor(u => u.UpdatedAt, f => f.Date.RecentOffset());
        }
    }
}
