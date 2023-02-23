using System.Text;
using MoneyHumanizer.Service.Extensions.BaseTypeExtensions;

namespace MoneyHumanizer.Service.Humanizers;


// SEE README.MD FOR EXPLANATION OF ALGORITHM STRATEGY
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

        var pluralizeDollars = dollarDigits.CombineToNumber() > 1 ? "dollars" : "dollar";

        var humanized = $"{sign}{HumanizeDigits(dollarDigits)} {pluralizeDollars}";

        // only add on cents if they're greater than zero ("and zero cents" is technically correct but we don't generally say it)
        if (centDigits.Sum() > 0)
        {
            var pluralizeCents = centDigits.CombineToNumber() > 1 ? "cents" : "cent";
            humanized += $" and {HumanizeDigits(centDigits)} {pluralizeCents}";
        }

        return humanized;
    }

    private string HumanizeNumbersLessThan100(int[] digits)
    {
        // re-calculate the actual number based in the supplied digits array for initial logic around numbers less than 100.
        var number = digits.CombineToNumber();

        // handle all uniquely-named numbers below twenty.
        if (number < 20)
            return DigitsAndTeens[number];

        // handle cases less than 100 but greater than 20.
        if (number < 100)
        {
            var tens = Tens[digits[0] - 1];

            // joining 1-9 digits with the tens number so return only the tens number if the number is 20, 30, 40, etc. to avoid attaching the zero on.
            if (digits[1] == 0)
                return tens;

            // Attach the remaining 0-9 digits.
            return string.Join('-', tens, DigitsAndTeens[digits[1]]);
        }

        throw new ArgumentOutOfRangeException($"Function can only humanize numbers less than 100. Provided input was: {number}");
    }

    private string HumanizeDigits(int[] digits)
    {        
        // Remove any left-padded zeros in the array
        var unpaddedDigits = digits
            .SkipWhile(digit => digit == 0)
            .ToArray();

        // if less than 100 return with separate logic. This is because many of these numbers are uniquely named compared to the others.
        //      e.g. 30 is "thirty" and not "three ten", despite the fact that 300 is "three hundred".
        //
        // IMPORTANT: this is a break condition for the subsequent recursive calls to this method.
        if(unpaddedDigits.Length < 3)
            return HumanizeNumbersLessThan100(unpaddedDigits);

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
            .SkipWhile(digit => digit == 0) // Once again remove any left-padding zeros for remaining digits
            .ToArray();

        // determine whether or not to append the word "and" as per common speech syntax if it is relevant to do so
        //  i.e. if there are remaining digits in the syntactically appropriate locations
        var remaining = remainingDigits.CombineToNumber();
        if ((0 < remaining & remaining < 100))
        {
            humanizedStringBuilder.Append(" and");
        }

        // Recursively append any remaining humanizable digits
        if (remaining > 0)
        {
            humanizedStringBuilder
                .Append(' ')
                .Append(
                    HumanizeDigits(remainingDigits)
                );
        }

        return humanizedStringBuilder.ToString();
    }
}
