using Microsoft.AspNetCore.Mvc;
using Humanizer;
using System.Text;
using NumericEnglishLanguageParser.Service.Humanizers;

namespace NumericEnglishLanguageParser.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class MoneyHumanizerController : ControllerBase
{
    private readonly IMoneyHumanizer _humanizer;

    public MoneyHumanizerController(IMoneyHumanizer humanizer)
    {
        _humanizer = humanizer;
    }

    [HttpGet(Name = "GetValueInEnglish")]
    public string Get(decimal value)
    {
        return _humanizer.Humanize(value);
    }

}
