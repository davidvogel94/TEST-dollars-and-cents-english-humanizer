using Humanizer;

using System.Globalization;
using System.Text;

namespace NumericEnglishLanguageParser.Service.Humanizers;

public class MoneyHumanizer : IMoneyHumanizer
{
    private readonly CultureInfo _cultureInfo;

    public MoneyHumanizer(CultureInfo? cultureInfo = null)
    {
        _cultureInfo = cultureInfo ?? CultureInfo.CurrentCulture;
    }

    public string Humanize(decimal value)
    {
        var dollars = Convert.ToInt32(Decimal.Truncate(value));

        // Make sure that the cents are unsigned so any possible negative isn't humanized twice.
        var cents = Math.Abs(
            Convert.ToInt32((value - dollars) * 100)
        );

        // Construct output
        var responseBuilder = new StringBuilder();
        responseBuilder.AppendJoin(' ', dollars.ToWords(_cultureInfo), "dollars");

        if (cents > 0) // Don't include cents in the humanization if the number is zero.
            responseBuilder.AppendJoin(' ', " and", cents.ToWords(_cultureInfo), "cents");

        return responseBuilder.ToString();
    }
}
