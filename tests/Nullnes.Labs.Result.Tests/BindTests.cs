using AwesomeAssertions;
using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class BindTests
{
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Binding")]
    [Theory(DisplayName = "Binding: Should chain successful results through bind operation")]
    public void Should_Bind_Success_When_Result_Is_Success(int value)
    {
        // Arrange
        Result<int, TestError> successfulResult = value;

        // Act
        Result<string, TestError> finalResult = successfulResult
            .Bind<string>(successValue => successValue.ToString());

        // Assert
        string result = finalResult.Match(
            onSuccess: s => s,
            onError: error => error.Message);

        result.Should().Be(value.ToString(), "bind should preserve success value through chain");
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Binding")]
    [Theory(DisplayName = "Binding: Should propagate error through bind operation")]
    public void Should_Bind_Error_When_Result_Is_Failure(int value)
    {
        // Arrange
        TestError expectedError = new TestError($"Error occurred with value {value}");
        Result<int, TestError> failureResult = expectedError;

        // Act
        Result<string, TestError> finalResult = failureResult.Bind(
            successValue =>
            {
                if (successValue == value)
                    return Result.Success<string, TestError>(successValue.ToString());
                return new TestError("This should never happen, because bind should preserve the previous error.");
            });

        // Assert
        string result = finalResult.Match(
            onSuccess: _ => "Success",
            onError: error => error.Message);

        result.Should().Be(expectedError.Message, "bind should preserve error message through chain");
    }
}