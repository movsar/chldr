using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.Models
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

        /*
         * @description checks if a number is within a range, lower inclusive and upper exclusive
         */
        public bool In(int number, NumericRange range)
        {
            return number >= range.Lower && number < range.Upper;
        }
    }
}
