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
        //private static NumericRange MemberRateRange = new Range(1, 10);
        //private static NumericRange EnthusiastRateRange = new Range(10, 50);
        //private static NumericRange ContributorRateRange = new Range(50, 500);
        //private static NumericRange EditorRateRange = new Range(500, 10000);
        //private static NumericRange MaintainerRateRange = new Range(10000, 500000000);
        //private static bool IsWithin(Range range, int value)
        //{
        //    return value >= range.Start && value <= maximum;
        //}
        public ObjectId UserId { get; }
        public string Email { get; }
        public int Rate { get; }
        public Rank Rank
        {
            get
            {
                if (Rate < 10)
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
            UserId = user._id;
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
