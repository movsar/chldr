using chldr_data.Enums;
using chldr_data.DatabaseObjects.DatabaseEntities;
using chldr_data.DatabaseObjects.Models;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class UserDto : IUser
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        [JsonIgnore]
        public string? PasswordConfirmation { get; set; }
        public int RateWeight { get; set; } = 1;
        public int Rate { get; set; } = 1;
        public string? Patronymic { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserStatus UserStatus { get; set; } = UserStatus.Unconfirmed;
        public string? ImagePath { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static UserDto FromModel(UserModel model)
        {
            return new UserDto()
            {
                UserId = model.UserId,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Rate = model.Rate,
            };
        }
    }
}
