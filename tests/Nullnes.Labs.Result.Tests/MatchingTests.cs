using AwesomeAssertions;

using Nullnes.Labs.Result.Tests.TestHelpers;
using Xunit;

namespace Nullnes.Labs.Result.Tests;

public sealed class MatchingTests
{
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Matching")]
    [Theory(DisplayName = "Matching: Should execute success function for successful result")]
    public void Should_Match_Success_When_Result_Is_Success(int value)
    {
        // Arrange
        Result<int, TestError> successfulResult = value;

        // Act
        string finalResult = successfulResult.Match(
            onSuccess: successResult => OnSuccess(successResult),
            onError: _ => "Error");

        // Assert
        finalResult.Should()
            .Be(OnSuccess(value), "success function should be executed for successful result");
        
        string OnSuccess(int input) => input.ToString();
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Matching")]
    [Theory(DisplayName = "Matching: Should execute error function for failed result")]
    public void Should_Match_Error_When_Result_Is_Failure(int value)
    {
        // Arrange
        TestError expectedError = new TestError($"Error occurred with value {value}");
        Result<int, TestError> failureResult = expectedError;

        // Act
        string finalResult = failureResult.Match(
            onSuccess: successResult => OnSuccess(successResult),
            onError: error => error.Message);

        // Assert
        finalResult.Should()
            .Be(expectedError.Message, "error function should be executed for failed result");
        
        string OnSuccess(int input) => input.ToString();
    }
}