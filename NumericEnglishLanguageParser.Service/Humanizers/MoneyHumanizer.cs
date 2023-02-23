using System.Text;
using NumericEnglishLanguageParser.Service.Extensions;

namespace NumericEnglishLanguageParser.Service.Humanizers;


/*
 *  General strategy: 
 *      1. Define "digit groups" corresponding to "hundred", "thousand", "million" etc. and associate them with the number of digits that any given number 
 *          must have to belong to these groups.
 *
 *      2. Account for digits corresponding to numbers less than 20 as these numbers have unique names in English compared to every other number
 *
 *      3. Remove any left-padded zeros from the input digit array
 *
 *      4. Account for cases where the number falls between 20 and 100, as the tens groupings also have unique names in English
 *      
 *      5. Using the number of digits minimally required for the digit grouping to which the number belongs, take away this many digits from
 *          the end of the number and humanize what remains using a recursive function call.
 *
 *          This is done to accomplish the humanization of, for example, the "one hundred" in "one hundred thousand".
 *
 *          Following this, append the actual digit grouping for the number, e.g. "thousand" in "one hundred thousand".
 * 
 *      6. Return the humanized string including a recursive function call to humanize any digits that remain to be humanized.
 */

public class MoneyHumanizer : IMoneyHumanizer
{
    public MoneyHumanizer() { }

    private static readonly string[] DigitsAndTeens = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
    private static readonly string[] Tens = new[] { "ten", "twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninety" };

    private sealed class DigitGroupNames
    {
        public const string SINGULAR = "singular";
        public const string TEEN = "teen";
        public const string HUNDRED = "hundred";
        public const string THOUSAND = "thousand";
        public const string MILLION = "million";
        public const string BILLION = "billion";
        public const string TRILLION = "trillion";
        public const string QUADRILLION = "quadrillion";
        public const string QUINTILLION = "quintillion";
        public const string GOOGOL = "googol";
    }

    private static readonly Dictionary<int, string> DigitGrouping = new Dictionary<int, string>
    {
        {1,     DigitGroupNames.SINGULAR},      // log(10^0) + 1 = 0 + 1 = 1   - plus one included to account for non-zero digit at front
        {2,     DigitGroupNames.TEEN},          // log(10^1) + 1 = 2
        {3,     DigitGroupNames.HUNDRED},       // ...
        {4,     DigitGroupNames.THOUSAND},      // ...
        {7,     DigitGroupNames.MILLION},       // log(10^6) + 1 = 7
        {10,    DigitGroupNames.BILLION},       // ...
        {13,    DigitGroupNames.TRILLION},      // ...
        {16,    DigitGroupNames.QUADRILLION},   // ...
        {19,    DigitGroupNames.QUINTILLION},   // ...
        {101,   DigitGroupNames.GOOGOL}         // log(10^100) + 1 = 101
    };


    public string Humanize(decimal value)
    {
        var sign = value < 0 ? "minus " : string.Empty;

        // use extension methods to obtain int[] arrays containing digits for the whole and fractional parts of the decimal.
        var dollarDigits = value.WholePartDigits();
        var centDigits = value.FractionPartDigits(decimalPlaces: 2);

        var pluralizeDollars = dollarDigits.DigitsToNumber() > 1 ? "dollars" : "dollar";

        var humanized = $"{sign}{HumanizeDigits(dollarDigits)} {pluralizeDollars}";

        // only add on cents if they're greater than zero ("and zero cents" is technically correct but we don't generally say it)
        if (centDigits.Sum() > 0)
        {
            var pluralizeCents = centDigits.DigitsToNumber() > 1 ? "cents" : "cent";
            humanized += $" and {HumanizeDigits(centDigits)} {pluralizeCents}";
        }

        return humanized;
    }

    private string HumanizeDigits(int[] digits)
    {
        // re-calculate the actual number based in the supplied digits array for initial logic around numbers less than 100.
        // N.B.: this is also the logic which will become the exit path for recursive function calls.
        var number = digits.DigitsToNumber();

        // handle all uniquely-named numbers below twenty.
        if (number < 20)
            return DigitsAndTeens[number];

        // Remove any left-padded zeros in the array
        var unpaddedDigits = digits
            .SkipWhile(digit => digit == 0)
            .ToArray();

        // handle cases less than 100 but greater than 20.
        if (unpaddedDigits.Length == 2)
        {
            var tens = Tens[unpaddedDigits[0] - 1];

            // joining 1-9 digits with the tens number so return only the tens number if the number is 20, 30, 40, etc. to avoid attaching the zero on.
            if (unpaddedDigits[1] == 0)
                return tens;

            // Attach the remaining 0-9 digits.
            return string.Join('-', tens, DigitsAndTeens[unpaddedDigits[1]]);
        }

        // Work out the next digit grouping, e.g. the 100 in 100,000
        var digitGroup = DigitGrouping.Keys.Last(i => i <= unpaddedDigits.Length);
        var digitsInGroup = unpaddedDigits
            .SkipLast(digitGroup - 1)
            .ToArray();

        // Recursive call to humanise only the digits belonging to the current digit group, e.g. the "one hundred" in "one hundred thousand"
        var humanizedStringBuilder = new StringBuilder(
            HumanizeDigits(digitsInGroup) 
        );

        // append the digit group word, e.g. the "thousand" in "one thousand"
        humanizedStringBuilder
            .Append(' ')
            .Append(DigitGrouping[digitGroup]);

        var remainingDigits = unpaddedDigits
            .Skip(digitsInGroup.Length)
            .SkipWhile(digit => digit == 0) // remove any left-padding zeros for remaining digits
            .ToArray();

        // determine whether or not to append the word "and" as per common speech syntax if it is relevant to do so
        //  i.e. if there are remaining digits in the syntactically appropriate locations
        var remaining = remainingDigits.DigitsToNumber();
        if ((0 < remaining & remaining < 100))
        {
            humanizedStringBuilder.Append(" and");
        }

        // Recursively append any remaining humanizable digits
        if (remaining > 0)
        {
            humanizedStringBuilder
                .Append(' ')
                .Append(HumanizeDigits(remainingDigits));
        }

        return humanizedStringBuilder.ToString();
    }
}
