# Test Plan
The test plan for the service - as it has been initially written - is fairly simple. Included unit testing should ensure that calls to the humanization code itself are not broken during any kind of code modification or extension, so what remains is end-to-end integration testing.

## Unit testing
A simple controller unit test has been written using the XUnit testing framework and NSubstitute for mocking the humanizer class itself. This test was written to ensure that the controller does in fact make calls to the humanizer code.

The benefit of having made use of `Autofac` for inversion of control is that controllers and other classes may be designed with automatic dependency injection, meaning it's very easy to initialize test subjects with mock dependencies in any unit test scenario.

## Integration testing
Given the simplicity of this project, it is difficult to write specific *unit* tests for the humanizer code itself as the unit test would need to somehow validate the algorithm being used independently and in a logic- rather than value-based test scenario. Because the nature of this code is essentially "parse input, process input, spit out result", integration tests are far easier to create, maintain, and use to verify program output, especially for A/B or end-to-end testing for when the code changes or is extended.

As such, it is important to perform integration tests with a variety of different inputs to represent the spectrum of different possible outputs that may be produced. [An integration test file](/REST%20client%20integration%20tests/integration-tests.json) has been included for use with a REST client such as [Thunder Client for VS Code](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client). It is noted that this kind of testing can also be easily automated for use in build pipelines.

## Convention Testing

It may be important in future iterations to include convention testing, however given this is a stand-alone project there are currently no organisational code conventions that are applicable.