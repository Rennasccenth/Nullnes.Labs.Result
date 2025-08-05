using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class CompositionTests
{
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Composition")]
    [Theory(DisplayName = "Composition: Should chain multiple Successful Synchronous operations")]
    public void Should_Chain_Multiple_Successful_Synchronous_Operations(int startValue)
    {
        // Arrange
        Result<int, TestError> initialResult = startValue;

        // Act
        var finalResult = initialResult
            .Map(TestFunctions.Double) // x2
            .Bind(TestFunctions.SimulateSuccess.DoubleResult) // x4
            .Map(TestFunctions.SubtractOne); // (x4) - 1

        string unwrappedValue = finalResult
            .Match(
                onSuccess: value => value.ToString(),
                onError: err => err.Message.ToString());

        // Assert
        int expectedNumber = TestFunctions.SubtractOne(TestFunctions.Double(TestFunctions.Double(startValue)));
        unwrappedValue.Should().Be(expectedNumber.ToString(), "all operations should be applied in sequence");
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Composition")]
    [Theory(DisplayName = "Composition: Should compose Sync and Async operations")]
    public async Task Should_Compose_Sync_And_Async_Operations(int startValue)
    {
        // Arrange
        Result<int, TestError> initialResult = startValue;

        // Act
        var finalResult = await initialResult
            .Map(TestFunctions.Double) // x2 
            .Bind(TestFunctions.SimulateSuccess.DoubleResultAsync) // x4
            .Map(TestFunctions.SubtractOneAsync); // (x4) - 1

        string unwrappedValue = finalResult.Match(
            onSuccess: value => value.ToString(),
            onError: err => err.Message.ToString());

        // Assert
        int expectedNumber = await TestFunctions
            .SubtractOneAsync(await TestFunctions
                .DoubleAsync(await TestFunctions
                    .DoubleAsync(startValue)));

        unwrappedValue.Should().Be(expectedNumber.ToString(), "operations should be composed regardless of Sync or Async nature");
    }
} 