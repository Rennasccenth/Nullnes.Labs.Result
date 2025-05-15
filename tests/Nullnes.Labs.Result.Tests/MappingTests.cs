using FluentAssertions;
using Nullnes.Labs.Result.Tests.TestHelpers;
using Xunit;

namespace Nullnes.Labs.Result.Tests;

public sealed class MappingTests
{
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(9999)]
    [InlineData(-9999)]
    [Trait("Category", "Mapping")]
    [Theory(DisplayName = "Mapping: Should map successful result with sync function")]
    public void Should_Map_Sync_On_Sync_SuccessResult(int expected)
    {
        // Arrange
        Result<int, TestError> result = 0;

        // Act
        Result<int, TestError> mappedResult = result.Map(_ => expected);

        string resultValue = mappedResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message.ToString());

        // Assert
        resultValue.Should().Be(expected.ToString(), "successful result should be mapped to new value");
    }

    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(9999)]
    [InlineData(-9999)]
    [Trait("Category", "Mapping")]
    [Theory(DisplayName = "Mapping: Should map successful result with async function")]
    public async Task Should_Map_Async_On_Sync_SuccessResult(int expected)
    {
        // Arrange
        Result<int, TestError> result = 0;

        // Act
        Result<int, TestError> mappedResult = await result.Map(async _ => await Task.FromResult(expected));

        string resultValue = mappedResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message.ToString());

        // Assert
        resultValue.Should().Be(expected.ToString(), "successful result should be mapped to new value");
    }

    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(9999)]
    [InlineData(-9999)]
    [Trait("Category", "Mapping")]
    [Theory(DisplayName = "Mapping: Should map async successful result with sync function")]
    public async Task Should_Map_Sync_On_Async_Success_Result(int successValue)
    {
        // Arrange
        Task<Result<int, TestError>> asyncResult = Task.FromResult<Result<int, TestError>>(successValue);

        int MultiplyByTwo(int x) => x * 2;

        // Act
        var mappedResult = await asyncResult.Map(MultiplyByTwo);

        string resultValue = mappedResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message.ToString());

        // Assert
        resultValue.Should().Be(MultiplyByTwo(successValue).ToString(), "async result should be mapped to new value");
    }

    [Trait("Category", "Mapping")]
    [Fact(DisplayName = "Mapping: Should preserve error in async failed result")]
    public async Task Should_Not_Map_Sync_On_Async_Failed_Result()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        Task<Result<int, TestError>> asyncResult = Task.FromResult<Result<int, TestError>>(new TestError(errorMessage));

        int MultiplyByTwo(int x) => x * 2;

        // Act
        var mappedResult = await asyncResult.Map(MultiplyByTwo);

        string resultValue = mappedResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message.ToString());

        // Assert
        resultValue.Should().Be(errorMessage, "error should be preserved through mapping");
    }

    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(9999)]
    [InlineData(-9999)]
    [Trait("Category", "Mapping")]
    [Theory(DisplayName = "Mapping: Should map async successful result with async function")]
    public async Task Should_Map_Async_On_Sync_Success_Result(int successValue)
    {
        // Arrange
        Task<Result<int, TestError>> asyncResult = Task.FromResult<Result<int, TestError>>(successValue);

        Task<int> MultiplyByTwoAsync(int x) => Task.FromResult(x * 2);

        // Act
        var mappedResult = await asyncResult.Map(MultiplyByTwoAsync);

        string resultValue = mappedResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message.ToString());

        // Assert
        int expectedValue = await MultiplyByTwoAsync(successValue);
        resultValue.Should().Be(expectedValue.ToString(), "async result should be mapped to new value");
    }

    [Trait("Category", "Mapping")]
    [Fact(DisplayName = "Mapping: Should preserve error in async failed result with async function")]
    public async Task Should_Not_Map_Async_On_Async_Failed_Result()
    {
        // Arrange
        const string expectedErrorMessage = "Error occurred";
        Task<Result<int, TestError>> asyncResult = Task.FromResult<Result<int, TestError>>(new TestError(expectedErrorMessage));

        Task<int> MultiplyByTwoAsync(int x) => Task.FromResult(x * 2);

        // Act
        var mappedResult = await asyncResult.Map(MultiplyByTwoAsync);

        string resultValue = mappedResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message.ToString());

        // Assert
        resultValue.Should().Be(expectedErrorMessage, "error should be preserved through async mapping");
    }

    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(9999)]
    [InlineData(-9999)]
    [Trait("Category", "Mapping")]
    [Theory(DisplayName = "Mapping: Should preserve error in sync failed result")]
    public void Should_Not_Map_Sync_On_Sync_Failed_Result(int expectedOnSuccess)
    {
        // Arrange
        const string expectedMessage = "Error occurred";
        Result<int, TestError> result = new TestError(expectedMessage);

        // Act
        Result<int, TestError> mappedResult = result.Map(_ => expectedOnSuccess);

        string resultValue = mappedResult.Match(
            onSuccess: successValue => successValue.ToString(),
            onError: error => error.Message);

        // Assert
        resultValue.Should().Be(expectedMessage, "error should be preserved through mapping");
    }

    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(9999)]
    [InlineData(-9999)]
    [Trait("Category", "Mapping")]
    [Theory(DisplayName = "Mapping: Should preserve error in sync failed result with async function")]
    public async Task Should_Not_Map_Async_On_Sync_Failed_Result(int expectedOnSuccess)
    {
        // Arrange
        const string expectedMessage = "Error occurred";
        Result<int, TestError> result = new TestError(expectedMessage);

        // Act
        Result<int, TestError> mappedResult = await result.Map(async _ => await Task.FromResult(expectedOnSuccess));

        string resultValue = mappedResult.Match(
            onSuccess: successValue => successValue.ToString(),
            onError: error => error.Message);

        // Assert
        resultValue.Should().Be(expectedMessage, "error should be preserved through async mapping");
    }
}
