using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
using Newtonsoft.Json;

namespace chldr_data.Dto
{
    public class UserDto : IUser
    {
        public UserDto(IUser user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Rate = user.Rate;
        }
        public UserDto() { }
        public string? Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string PasswordConfirmation { get; set; }
        public int RateWeight { get; set; }
        public int Rate { get; set; }
        public string? Patronymic { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
