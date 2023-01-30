using chldr_data.Enums;
using chldr_shared.Models;
using MongoDB.Bson;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models
{
    public class UserModel
    {
        // Members can only add and vote, they'll get their rate increased when moders approve their entries
        public static NumericRange MemberRateRange = new NumericRange(1, 10);
        public static NumericRange EnthusiastRateRange = new NumericRange(10, 50);
        // Initial Rate of official dictionary entries = 50, so that only contributors can edit them
        public static NumericRange ContributorRateRange = new NumericRange(50, 500);
        public static NumericRange EditorRateRange = new NumericRange(500, 10000);
        public static NumericRange MaintainerRateRange = new NumericRange(10000, 500000000);
        public ObjectId UserId { get; }
        public string? Email { get; }
        public int Rate { get; }
        public Rank Rank { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? Patronymic { get; }
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
            Rank = GetRankByRate(user.Rate);
        }

        private Rank GetRankByRate(int rate)
        {
            if (MemberRateRange.Contains(rate))
            {
                return Rank.Member;
            }
            else if (EnthusiastRateRange.Contains(rate))
            {
                return Rank.Enthusiast;
            }
            else if (ContributorRateRange.Contains(rate))
            {
                return Rank.Contributor;
            }
            else if (EditorRateRange.Contains(rate))
            {
                return Rank.Editor;
            }
            else if (MaintainerRateRange.Contains(rate))
            {
                return Rank.Maintainer;
            }
            else
            {
                return Rank.Member;
            }
        }
        public bool CanEditEntry(EntryModel entry)
        {
            if (Rank >= GetRankByRate(entry.Rate))
            {
                return true;
            }

            return false;
        }
        public bool CanEditTranslation(TranslationModel translation)
        {
            if (Rank >= GetRankByRate(translation.Rate))
            {
                return true;
            }

            return false;
        }
    }
}
