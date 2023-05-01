using chldr_data.Entities;
using chldr_tools;
using Microsoft.EntityFrameworkCore;

namespace chldr_api
{
    public class Mutation
    {
        [UseDbContext(typeof(SqlContext))]
        public async Task<SqlUser> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic, [ScopedService] SqlContext dbContext)
        {
            // Check if a user with this email already exists
            var existingUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new GraphQLException("A user with this email already exists");
            }

            // Create the new user
            var user = new SqlUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return user;
        }

        [UseDbContext(typeof(SqlContext))]
        public async Task<SqlUser> LoginUserAsync(string email, string password, [ScopedService] SqlContext dbContext)
        {
            // Check if a user with this email exists
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new GraphQLException("No user found with this email");
            }

            // Check if the password is correct
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new GraphQLException("Incorrect password");
            }

            return user;
        }
    }
}