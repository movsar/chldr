using chldr_data.Enums;
using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Models;
using Realms.Sync;
using Newtonsoft.Json;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class UserModel : IUser
    {
        [JsonConstructor]
        private UserModel() { }
        // Members can only add new entries and translations they'll get their rate increased when moders approve their entries
        public static NumericRange MemberRateRange = new NumericRange(1, 10);
        public static NumericRange EnthusiastRateRange = new NumericRange(10, 100);
        public static NumericRange ContributorRateRange = new NumericRange(100, 1000);
        public static NumericRange EditorRateRange = new NumericRange(1000, 10000);
        public static NumericRange MaintainerRateRange = new NumericRange(10000, 500000000);
        public string? Email { get; set; }
        public int Rate { get; set; }
        public RateWeight RateWeight { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string? ImagePath { get; set; }

        public NumericRange GetRateRange()
        {
            if (MemberRateRange.Contains(Rate))
            {
                return MemberRateRange;
            }
            else if (EnthusiastRateRange.Contains(Rate))
            {
                return EnthusiastRateRange;
            }
            else if (ContributorRateRange.Contains(Rate))
            {
                return ContributorRateRange;
            }
            else if (EditorRateRange.Contains(Rate))
            {
                return EditorRateRange;
            }
            else if (MaintainerRateRange.Contains(Rate))
            {
                return MaintainerRateRange;
            }
            else
            {
                return MemberRateRange;
            }
        }

        public static RateWeight GetRateWeightByRate(int rate)
        {
            if (MemberRateRange.Contains(rate))
            {
                return RateWeight.Member;
            }
            else if (EnthusiastRateRange.Contains(rate))
            {
                return RateWeight.Enthusiast;
            }
            else if (ContributorRateRange.Contains(rate))
            {
                return RateWeight.Contributor;
            }
            else if (EditorRateRange.Contains(rate))
            {
                return RateWeight.Editor;
            }
            else if (MaintainerRateRange.Contains(rate))
            {
                return RateWeight.Maintainer;
            }
            else
            {
                return RateWeight.Member;
            }
        }
        public bool CanEditEntry(EntryModel entry)
        {
            if (RateWeight >= GetRateWeightByRate(entry.Rate))
            {
                return true;
            }

            return false;
        }
        public bool CanEditTranslation(TranslationModel translation)
        {
            if (RateWeight >= GetRateWeightByRate(translation.Rate))
            {
                return true;
            }

            return false;
        }   

        private static UserModel FromBaseInterface(IUser user)
        {
            if (string.IsNullOrWhiteSpace(user.UserId))
            {
                throw new NullReferenceException("UserId is null");
            }

            return new UserModel()
            {
                UserId = user.UserId,
                Email = user.Email,
                Rate = user.Rate,
                ImagePath = user.ImagePath,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                RateWeight = GetRateWeightByRate(user.Rate)
            };
        }

        public static UserModel FromDto(UserDto? userDto)
        {
            var userModel = FromBaseInterface(userDto);
            // Specific dto fields
            return userModel;
        }

        public static UserModel FromEntity(IUserEntity? userEntity)
        {
            var userModel = FromBaseInterface(userEntity);
            // Specific entity fields
            return userModel;
        }
    }
}
