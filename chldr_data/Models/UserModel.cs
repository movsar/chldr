using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models
{
    public class UserModel
    {
        public ObjectId EntityId { get; }
        public string Email { get; }
        public int RateWeight { get; }
        public int Rate { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Patronymic { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }

        public UserModel(Entities.User user)
        {
            EntityId = user._id;
            Email = user.Email;
            RateWeight = user.RateWeight;
            Rate = user.Rate;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Patronymic = user.Patronymic;
            CreatedAt = user.CreatedAt;
            UpdatedAt = user.UpdatedAt;
        }
    }
}
