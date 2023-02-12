using chldr_data.Enums;
using chldr_shared.Models;

namespace chldr_data.Models
{
    public class UserModel : ModelBase
    {
        // Members can only add new entries and translations they'll get their rate increased when moders approve their entries
        public static NumericRange MemberRateRange = new NumericRange(1, 10);
        public static NumericRange EnthusiastRateRange = new NumericRange(10, 100);
        public static NumericRange ContributorRateRange = new NumericRange(100, 1000);
        public static NumericRange EditorRateRange = new NumericRange(1000, 10000);
        public static NumericRange MaintainerRateRange = new NumericRange(10000, 500000000);
        public string? Email { get; }
        public int Rate { get; }
        public Rank Rank { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? Patronymic { get; }

        public UserModel(Entities.User user) : base(user)
        {
            Email = user.Email;
            Rate = user.Rate;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Patronymic = user.Patronymic;
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
