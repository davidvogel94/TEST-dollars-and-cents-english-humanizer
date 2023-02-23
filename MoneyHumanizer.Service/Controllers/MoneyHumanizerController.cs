using Microsoft.AspNetCore.Mvc;
using MoneyHumanizer.Service.Humanizers;

namespace MoneyHumanizer.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class MoneyHumanizerController : ControllerBase
{
    private readonly IMoneyHumanizer _humanizer;

    public MoneyHumanizerController(IMoneyHumanizer humanizer)
    {
        _humanizer = humanizer;
    }

    /// <summary>
    /// Converts a dollars-and-cents value into humanized English word format
    /// </summary>
    /// <param name="value">The decimal currency value to be humanized</param>
    /// <returns>A humanized English-language form of the input value</returns>
    /// <remarks>
    /// The returned value is a raw string; it has not been wrapped in any JSON formatting.
    /// </remarks>
    /// <response code="200">Humanization succeeded</response>
    /// <response code="400">Bad request: most likely the provided value parameter contains letters or symbols and thus doesn't convert into a decimal value.</response>
    /// <response code="500">Humanization failed due to unhandled error in the API code</response>
    [HttpGet(Name = "GetValueInEnglish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public string Get(decimal value)
    {
        return _humanizer.Humanize(value);
    }

}
