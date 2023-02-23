using MoneyHumanizer.Service.Controllers;
using MoneyHumanizer.Service.Humanizers;
using NSubstitute;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace MoneyHumanizer.Tests;

public class MoneyHumanizerControllerTests
{
    /*
     *      SUBJECT AND DEPENDENCY DECLARATIONS
     */
    private MoneyHumanizerController _subject;
    private IMoneyHumanizer _humanizer;

    /*
     *      TEST DATA
     */
    private const decimal MoneyHumanizerMockInputValue = (decimal)1523428.56;
    private const string MoneyHumanizerMockReturnValue = "one million five hundred and twenty-three thousand four hundred and twenty-eight dollars and fifty-six cents";

    /*
     *      OUTPUT DECLARATIONS
     */
    private Exception? _exception;
    private string? responseValue;

    // ---

    public MoneyHumanizerControllerTests()
    {
        // Initialize mocks
        _humanizer = Substitute.For<IMoneyHumanizer>();
        // Initialize test subject
        _subject = new MoneyHumanizerController(_humanizer);
    }

    [Fact]
    public void ItShouldHumanizeDollarsAndCents()
    {
        this.Given(_ => GivenMoneyHumanizerReturnsAValue())
            .When(_ => WhenControllerGetRequestIsReceived())
            .Then(_ => ThenTheInputMoneyValueIsHumanized())
            .And(_ => ThenTheControllerOutputShouldBeTheHumanizerOutput())
            .And(_ => ThenThereIsNoException())
            .BDDfy();
    }

    private void GivenMoneyHumanizerReturnsAValue()
    {
        // This is purely to stop any exceptions from occuring in the controller we're testing due to null reference etc.
        _humanizer
            .Humanize(Arg.Is<decimal>(MoneyHumanizerMockInputValue))
            .Returns(MoneyHumanizerMockReturnValue);
    }

    private void WhenControllerGetRequestIsReceived()
    {
        try 
        {
            responseValue = _subject.Get(MoneyHumanizerMockInputValue);
        }
        catch(Exception ex)
        {
            _exception = ex;
        }
    }

    private void ThenTheInputMoneyValueIsHumanized()
    {
        // Make sure a call to _humanizer.Humanize() was actually made.
        _humanizer.Received(1).Humanize(Arg.Is<decimal>(MoneyHumanizerMockInputValue));
        responseValue.ShouldNotBeNull();
    }

    private void ThenTheControllerOutputShouldBeTheHumanizerOutput()
    {
        // The controller was designed only to output the humanizer implementation's output without any additional markup or formatting.
        responseValue.ShouldBe(MoneyHumanizerMockReturnValue);
    }

    private void ThenThereIsNoException()
    {
        _exception.ShouldBeNull();
    }
}
