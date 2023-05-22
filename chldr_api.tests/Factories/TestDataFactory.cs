using chldr_data.DatabaseObjects.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_api.tests.Factories
{
    internal static class TestDataFactory
    {
        public static UserDto CreateUser()
        {
            return new UserDto()
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Patronymic = "Smith",
                Password = "Passw"
            };
        }
    }
}
