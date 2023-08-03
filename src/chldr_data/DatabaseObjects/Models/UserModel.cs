using chldr_data.Enums;
using chldr_data.DatabaseObjects.Dtos;
using Newtonsoft.Json;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Models;

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
        public UserRole Role => GetUserRole(Rate);
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string UserId { get; set; }
        public bool IsModerator { get; set; }
        public UserStatus Status { get; set; }
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
        public UserRole GetUserRole()
        {
            return GetUserRole(Rate);
        }
        public static UserRole GetUserRole(int rate)
        {
            if (MemberRateRange.Contains(rate))
            {
                return UserRole.Member;
            }
            else if (EnthusiastRateRange.Contains(rate))
            {
                return UserRole.Enthusiast;
            }
            else if (ContributorRateRange.Contains(rate))
            {
                return UserRole.Contributor;
            }
            else if (EditorRateRange.Contains(rate))
            {
                return UserRole.Editor;
            }
            else if (MaintainerRateRange.Contains(rate))
            {
                return UserRole.Maintainer;
            }
            else
            {
                return UserRole.Member;
            }
        }
        #region Permissions
        public bool CanAddSound(EntryDto entry)
        {
            if (Status != UserStatus.Active)
            {
                return false;
            }

            if (entry.SoundDtos.Where(s => s.Rate > 0).Count() >= Constants.MaxSoundsPerEntry)
            {
                return false;
            }

            return true;
        }
        public bool CanAddTranslation(EntryDto entry, string languageCode)
        {
            if (Status != UserStatus.Active)
            {
                return false;
            }

            if (entry.TranslationsDtos.Where(t => t.LanguageCode.Equals(languageCode) && t.Rate > MemberRateRange.Upper).Count() >= Constants.MaxTranslationsPerEntry)
            {
                return false;
            }

            return true;
        }
        public bool CanEdit(int entityRate, string entityUserId)
        {
            if (Status != UserStatus.Active || (Role <= GetUserRole(entityRate) && !UserId.Equals(entityUserId)))
            {
                return false;
            }

            return true;
        }
        public bool CanRemove(int entityRate, string entityUserId, DateTimeOffset entityCreatedAt)
        {
            // Removal is allowed only for the authors and only within X hours and for moderators
            if (Status != UserStatus.Active || (Role <= GetUserRole(entityRate) && !UserId.Equals(entityUserId)))
            {
                return false;
            }

            var timePassed = DateTimeOffset.UtcNow - entityCreatedAt;
            if ((timePassed.Hours < Constants.TimeInHrsToAllowForEntryRemoval) || IsModerator)
            {
                return true;
            }

            return false;
        }
        #endregion

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
            };
        }

        public static UserModel FromDto(UserDto? userDto)
        {
            var userModel = FromBaseInterface(userDto);
            userModel.Status = userDto.Status;
            return userModel;
        }

        public static UserModel FromEntity(IUserEntity? userEntity)
        {
            var userModel = FromBaseInterface(userEntity);
            userModel.Status = (UserStatus)userEntity.Status;
            return userModel;
        }

        public bool CanPromote(int rate, string userId)
        {
            if (Status != UserStatus.Active || UserId.Equals(userId) || Role <= GetUserRole(rate))
            {
                return false;
            }

            return true;
        }
    }
}
