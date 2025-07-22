using AwesomeAssertions;
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
    [Theory(DisplayName = "Composition: Should chain multiple successful operations")]
    public void Should_Chain_Multiple_Successful_Operations(int startValue)
    {
        // Arrange
        Result<int, TestError> initialResult = startValue;

        // Act
        var finalResult = initialResult
            .Map(AddOne)
            .Bind(MultiplyByTwoAsResult)
            .Map(SubtractThree);

        string resultValue = finalResult.Match(
            onSuccess: value => value.ToString(),
            onError: err => err.Message.ToString());

        // Assert
        string expected = SubtractThree(MultiplyByTwo(AddOne(startValue))).ToString();
        resultValue.Should().Be(expected, "all operations should be applied in sequence");

        static int AddOne(int x) => x + 1;
        static int MultiplyByTwo(int x) => x * 2;
        static Result<int, TestError> MultiplyByTwoAsResult(int x) => MultiplyByTwo(x);
        static int SubtractThree(int x) => x - 3;
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Composition")]
    [Theory(DisplayName = "Composition: Should compose sync and async operations")]
    public async Task Should_Compose_Sync_And_Async_Operations(int startValue)
    {
        // Arrange
        Result<int, TestError> initialResult = startValue;

        // Act
        var finalResult = await initialResult
            .Map(AddOne)
            .Bind(async x => await Task.FromResult<Result<int, TestError>>(MultiplyByTwo(x)))
            .Map(SubtractThree);

        string resultValue = finalResult.Match(
            onSuccess: value => value.ToString(),
            onError: err => err.Message.ToString());

        // Assert
        string expected = SubtractThree(MultiplyByTwo(AddOne(startValue))).ToString();
        resultValue.Should().Be(expected, "operations should be composed regardless of sync/async nature");

        static int AddOne(int x) => x + 1;
        static int MultiplyByTwo(int x) => x * 2;
        static int SubtractThree(int x) => x - 3;
    }
} 