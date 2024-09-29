using domain;
using domain.DatabaseObjects.Models;
using Newtonsoft.Json;
using domain.DatabaseObjects.Interfaces;
using domain.Enums;

namespace domain.DatabaseObjects.Dtos
{
    public class UserDto : IUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Email { get; set; }
        [JsonIgnore] public string? Password { get; set; }
        [JsonIgnore] public string? PasswordConfirmation { get; set; }
        public int Rate { get; set; } = 1;
        public string? Patronymic { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImagePath { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Unconfirmed;
        public UserType Type { get; set; } = UserType.Regular;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static UserDto FromModel(UserModel model)
        {
            return new UserDto()
            {
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                ImagePath = model.ImagePath,
                Patronymic = model.Patronymic,
                Id = model.Id,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Rate = model.Rate,
                Status = model.Status
            };
        }
    }
}
