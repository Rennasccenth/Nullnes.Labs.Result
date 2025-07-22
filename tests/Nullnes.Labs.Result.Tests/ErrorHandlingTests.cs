using AwesomeAssertions;

using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class ErrorHandlingTests
{
    [Fact(DisplayName = "Error Handling: Should propagate first error in chain")]
    [Trait("Category", "ErrorHandling")]
    public void Should_Propagate_First_Error_In_Chain()
    {
        // Arrange
        Result<int, TestError> initialResult = 42;
        TestError validationError = new TestError("Validation failed");
        TestError processingError = new TestError("Processing failed");

        // Act
        Result<int, TestError> finalResult = initialResult
            .Bind(ValidateNumber)
            .Bind(ProcessNumber);

        string errorMessage = finalResult.Match(
            onSuccess: _ => "Success",
            onError: error => error.Message);

        // Assert
        errorMessage.Should().Be(validationError.Message, "first error in chain should be propagated");

        Result<int, TestError> ValidateNumber(int x) => x > 40 ? validationError : x;
        Result<int, TestError> ProcessNumber(int x) => x < 20 ? processingError : x;
    }

    [Fact(DisplayName = "Error Handling: Should recover from error using fallback")]
    [Trait("Category", "ErrorHandling")]
    public void Should_Recover_Using_Fallback()
    {
        // Arrange
        Result<int, TestError> initialResult = 42;
        const int fallbackValue = 10;

        // Act
        int finalResult = initialResult
            .Bind(OperationThatFails)
            .Map(x => x * 2)
            .Match(
                onSuccess: x => x,
                onError: _ => fallbackValue);

        // Assert
        finalResult.Should().Be(fallbackValue, "should use fallback value when operation fails");

        static Result<int, TestError> OperationThatFails(int someParam) => new TestError($"Operation failed {someParam}");
    }

    [Fact(DisplayName = "Error Handling: Should short-circuit on first error")]
    [Trait("Category", "ErrorHandling")]
    public void Should_Short_Circuit_On_Error()
    {
        // Arrange
        Result<int, TestError> initialResult = 42;
        TestError expectedError = new TestError("Operation failed");

        // Act
        Result<int, TestError> finalResult = initialResult
            .Bind(FailOperation)
            .Map(ShouldNotExecute);

        string errorMessage = finalResult.Match(
            onSuccess: _ => "Success",
            onError: error => error.Message);

        // Assert
        errorMessage.Should().Be(expectedError.Message, "should stop execution at first error");

        static Result<int, TestError> FailOperation(int someParam) => new TestError($"Operation failed {someParam}");
        static int ShouldNotExecute(int someParam) => throw new InvalidOperationException($"This should not be executed {someParam}");
    }

    [Fact(DisplayName = "Error Handling: Should recover from async error using fallback")]
    [Trait("Category", "ErrorHandling")]
    public async Task Should_Recover_From_Async_Error()
    {
        // Arrange
        Result<int, TestError> initialResult = 42;
        int fallbackValue = 10;

        // Act
        Result<int, TestError> finalResult = await initialResult
            .Bind(AsyncOperationThatFails)
            .Map(x => x * 2);

        int result = finalResult.Match(
            onSuccess: x => x,
            onError: _ => fallbackValue);

        // Assert
        result.Should().Be(fallbackValue, "should use fallback value when async operation fails");

        static async Task<Result<int, TestError>> AsyncOperationThatFails(int someParam)
        {
            await Task.Delay(1);
            return new TestError($"Async operation failed {someParam}");
        }
    }
} 