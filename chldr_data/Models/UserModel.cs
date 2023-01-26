using chldr_data.Enums;
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
        public int Rate { get; }
        public Rank Rank
        {
            get
            {
                if (Rate < 9)
                {
                    // 1 - 9
                    return Rank.Member;
                }
                else if (Rate < 49)
                {
                    // 10 - 49
                    return Rank.Enthusiast;
                }
                else if (Rate < 499)
                {
                    // 50 - 499
                    return Rank.Contributor;
                }
                else if (Rate < 9999)
                {
                    // 500 - 9999 
                    return Rank.Editor;
                }
                else if (Rate >= 9999)
                {
                    // 9999 ->
                    return Rank.Maintainer;
                }
                return Rank.Member;
            }
        }
        public string FirstName { get; }
        public string LastName { get; }
        public string Patronymic { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }

        public UserModel(Entities.User user)
        {
            EntityId = user._id;
            Email = user.Email;
            Rate = user.Rate;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Patronymic = user.Patronymic;
            CreatedAt = user.CreatedAt;
            UpdatedAt = user.UpdatedAt;
        }
    }
}
