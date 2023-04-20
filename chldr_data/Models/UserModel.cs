using chldr_data.Enums;
using chldr_shared.Models;

namespace chldr_data.Models
{
    public class UserModel : PersistentModelBase
    {
        // Members can only add new entries and translations they'll get their rate increased when moders approve their entries
        public static NumericRange MemberRateRange = new NumericRange(1, 10);
        public static NumericRange EnthusiastRateRange = new NumericRange(10, 100);
        public static NumericRange ContributorRateRange = new NumericRange(100, 1000);
        public static NumericRange EditorRateRange = new NumericRange(1000, 10000);
        public static NumericRange MaintainerRateRange = new NumericRange(10000, 500000000);
        public string? Email { get; }
        public int Rate { get; }
        public RateWeight RateWeight { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? Patronymic { get; }

        public UserModel(Entities.SqlUser user) : base(user)
        {
            Email = user.Email;
            Rate = user.Rate;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Patronymic = user.Patronymic;
            RateWeight = GetRateWeight();
        }

        public RateWeight GetRateWeight()
        {
            return GetRateWeightByRate(Rate);
        }
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

    }
}
