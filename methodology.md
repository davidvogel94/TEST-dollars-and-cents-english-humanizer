# Methodology
## ASP.NET Rest API
A REST API with Swashbuckle was decided to be used as it fit the criteria of providing a web interface to allow for interactive testing while also demonstrating how the code may be implemented in a more realistic scenario (i.e. a microservice to be integrated with).

One of the criteria was to produce an interface that I would consider suitable for a customer, however given that the customer is undefined I have taken the liberty to assume that the customer is an API integration partner who will be interfacing with my code via REST calls or via Swagger documentation. The included Swagger UI further facilitates easy interaction and integration in this case.

## Algorithm Design
The service was architected making use of the [Humanizer](https://www.nuget.org/packages/Humanizer) library from NuGet. Once the application design was completed, the algorithm was designed based on the following:
### Assumptions
1. English-language only. No localisation required.
1. Dollars-and-cents only. No pounds, yen, or other currencies.

### Approach
1. After giving it some thought, it was realised that numbers as spoken in the English language tend to be grouped in accordance to groups:
    * Singular (0 - 9)
    * Teens (10 - 19)
    * Tens (20 - 90)
    * Hundreds (100 - 900)
    * Thousands (1,000 - 999,000)
    * Millions (1,000,000 - 999,000,000)
    * etc.

    Notice that after the hundreds, all subsequent groupings occur on a regular and repeatable basis.

1. It was decided to handle the small numbers (`singular` through to all `tens`, i.e. 0 - 99) separately to the rest of the input. This additionally provides a convenient exit condition for recursive function calls.
1. Removing any zeros padding the left-hand-side of the input (as they are not spoken in English), humanize a given digit grouping, then select the remaining digits and humanize them in turn with recursive function calls.

While the algorithm could be designed without recursion, the benefit of using recursion is that it is fast in its execution (relying more heavily on the stack rather than allocating array addresses to the heap) and does not implement any complex looping or array manipulation to achieve the same outcome, which could significantly increase the time complexity depending on how the arrays are manipulated.

Additionally, the recursion simplifies the code somewhat, as it considers individual digit groupings on a group-by-group basis, rather than trying to process all of the digit groups all at once.

Finally, recursion was deemed to be safe to use in this scenario as the input required to achieve a stack overflow is far higher than any conceivable use-case of this API.