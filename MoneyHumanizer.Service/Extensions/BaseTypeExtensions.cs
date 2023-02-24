namespace MoneyHumanizer.Service.Extensions.BaseTypeExtensions
{
    public static class DecimalExtensions
    {
        // using Math.Abs() as these extensions are explicitly for returning digits, not signs.

        public static int[] WholePartDigits(this decimal value)
        {
            var absValue = Math.Abs(value);

            return decimal
                .Truncate(absValue)
                .ToString()
                .Select(d => int.Parse(d.ToString()))
                .ToArray();
        }

        // Round off digits for the fraction part if specified; default to 28 - i.e. decimal max precision - to include all of the digits without rounding.
        public static int[] FractionPartDigits(this decimal value, int decimalPlaces = 28)
        {
            var absValue = Math.Abs(value);
            var fractionPart = Math.Round(absValue - decimal.Truncate(absValue), decimalPlaces);

            var fractionPartDigits = fractionPart
                .ToString()
                .Skip(2) // i.e. the '0.' part
                .Select(d => int.Parse(d.ToString()));

            if(fractionPartDigits.Count() < decimalPlaces)
            {
                foreach(var zero in Enumerable.Repeat(0, decimalPlaces - fractionPartDigits.Count()))
                    fractionPartDigits = fractionPartDigits.Append(zero);
            }

            return fractionPartDigits.ToArray();
        }
    }
    
    public static class IntArrayExtensions
    {
        public static long CombineToNumber(this int[] digits)
        {
            long returnVal = 0;

            for(int i = digits.Length; i > 0; i--)
                returnVal += Convert.ToInt64(Math.Pow(10, i-1) * digits[^i]);

            return returnVal;
        }
    }
}
