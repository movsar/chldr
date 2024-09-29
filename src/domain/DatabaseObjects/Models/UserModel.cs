using domain;
using domain.DatabaseObjects.Dtos;
using Newtonsoft.Json;
using domain.DatabaseObjects.Interfaces;
using domain.Models;
using domain;
using domain.Enums;

namespace domain.DatabaseObjects.Models
{
    public class UserModel : IUser
    {
        public UserModel() { }
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
        public string Id { get; set; }
        public UserType Type { get; set; } = UserType.Regular;
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

            if (entry.Sounds.Where(s => s.Rate > 0).Count() >= Constants.MaxSoundsPerEntry)
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

            if (entry.Translations.Where(t => t.LanguageCode.Equals(languageCode) && t.Rate > MemberRateRange.Upper).Count() >= Constants.MaxTranslationsPerEntry)
            {
                return false;
            }

            return true;
        }
        public bool CanEdit(int entityRate, string entityUserId)
        {
            if (Status != UserStatus.Active || (Role <= GetUserRole(entityRate) && !Id.Equals(entityUserId)))
            {
                return false;
            }

            return true;
        }
        public bool CanRemove(int entityRate, string entityUserId, DateTimeOffset entityCreatedAt)
        {
            // Removal is allowed only for the authors and only within X hours and for moderators
            if (Status != UserStatus.Active || (Role <= GetUserRole(entityRate) && !Id.Equals(entityUserId)))
            {
                return false;
            }

            var timePassed = DateTimeOffset.UtcNow - entityCreatedAt;
            if ((timePassed.Hours < Constants.TimeInHrsToAllowForEntryRemoval) || Type == UserType.Moderator)
            {
                return true;
            }

            return false;
        }
        #endregion

        private static UserModel FromBaseInterface(IUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Id))
            {
                throw new NullReferenceException("UserId is null");
            }

            return new UserModel()
            {
                Id = user.Id,
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
            if (userDto == null)
            {
                throw new NullReferenceException("UserDto is null");
            }

            var userModel = FromBaseInterface(userDto);
            userModel.Status = userDto.Status;
            userModel.Type = userDto.Type;
            return userModel;
        }

        public static UserModel FromEntity(IUserEntity? userEntity)
        {
            if (userEntity == null)
            {
                throw new NullReferenceException("UserEntity is null");
            }

            var userModel = FromBaseInterface(userEntity);
            userModel.Status = (UserStatus)userEntity.Status;
            userModel.Type = (UserType)userEntity.Type;
            return userModel;
        }

        public bool CanPromote(int rate, string userId)
        {
            if (Status != UserStatus.Active || Id.Equals(userId) || Role <= GetUserRole(rate))
            {
                return false;
            }

            return true;
        }

        public string GetFullName()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                return "";
            }
            else if (string.IsNullOrWhiteSpace(LastName))
            {
                return FirstName;
            }
            else
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
