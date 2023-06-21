namespace MoneyHumanizer.Service.Extensions.BaseTypeExtensions
{
    public static class DecimalExtensions
    {
        // using Math.Abs() as these extensions are explicitly for returning digits, not signs.
        public static int[] WholePartDigits(this decimal value) => Convert.ToInt64(decimal.Truncate(Math.Abs(value)))
            .GetDigitArray();

        // Round off digits for the fraction part if specified; default to 28 - i.e. decimal max precision - to include all of the digits without rounding.
        public static int[] FractionPartDigits(this decimal value, int decimalPlaces = 28)
        {
            var absValue = Math.Abs(value);
            var rounded = Math.Round(absValue - decimal.Truncate(absValue), decimalPlaces);

            for (var i = 0; i < decimalPlaces; i++) rounded *= 10;

            return Convert.ToInt64(rounded)
                .GetDigitArray();
        }
    }

    public static class IntArrayExtensions
    {
        public static int[] GetDigitArray(this long value) =>
            _enumerateDigitsInReverse(Math.Abs(value))
                .Reverse()
                .ToArray();

        private static IEnumerable<int> _enumerateDigitsInReverse(long i)
        {
            do
            {
                var shift = (i / 10) * 10; // make use of integer precision loss to get the last digit in i
                yield return (int)(i - shift);
            } while ((i /= 10) > 0);
        }

        public static long CombineToNumber(this int[] digits)
        {
            var returnVal = 0L;

            for (var i = digits.Length; i > 0; i--)
                returnVal += (long)Math.Pow(10, i - 1) * digits[^i];

            return returnVal;
        }
    }
}
