namespace domain.Models
{
    public class NumericRange
    {
        public int Lower { get; set; }
        public int Upper { get; set; }

        public NumericRange(int lower, int upper)
        {
            Lower = lower;
            Upper = upper;
        }

        /** @description checks if a number is within a range, lower inclusive and upper exclusive */
        public bool Contains(int number)
        {
            return number >= Lower && number < Upper;
        }
    }
}
