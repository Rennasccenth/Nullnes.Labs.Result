using AwesomeAssertions.Execution;
using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

[Trait("Category", "Binding")]
public sealed class BindTests
{
    private readonly Faker _faker = new();
    private readonly int _randomInteger;
    private readonly Result<int, TestError> _successfulResult;
    private readonly Task<Result<int, TestError>> _successfulAsyncResult;
    private readonly Result<int, TestError> _failureResult;
    private readonly Task<Result<int, TestError>> _failureResultAsync;

    public BindTests()
    {
        _randomInteger = _faker.Random.Int(-999_999, 999_999);
        
        _successfulResult = Result.Success<int, TestError>(_randomInteger);
        _successfulAsyncResult = Task.FromResult(_successfulResult);

        _failureResult = Result.Failure<int, TestError>(TestFunctions.DefaultError);
        _failureResultAsync = Task.FromResult(_failureResult);
    }

    [Fact(DisplayName = "Binding: Should execute Synchronous binding function over Synchronous SUCCESSFUL result")]
    public void Should_Bind_Next_Sync_Func_On_Sync_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = _successfulResult // Synchronously Succeeded
            .Bind(randomInteger => TestFunctions.SimulateSuccess.DoubleResult(randomInteger)) // Synchronously Succeeded
            .Bind(doubledValue => TestFunctions.SimulateSuccess.DoubleResult(doubledValue)); // Synchronously Bound to a success

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: TestFunctions.SimulateException.ThrowsAsInt);

        // Assert
        finalResult.Should().Be(_randomInteger * 2 * 2, "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should execute Asynchronous binding function over Synchronous SUCCESSFUL result")]
    public async Task Should_Bind_Next_Async_Func_On_Sync_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _successfulResult
            .Bind(randomInteger => TestFunctions.SimulateSuccess.DoubleResult(randomInteger)) // Synchronously Succeeds
            .Bind(doubledValue => TestFunctions.SimulateSuccess.DoubleResultAsync(doubledValue)); // Asynchronously Bound to a Synchronous Success

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: TestFunctions.SimulateException.ThrowsAsInt);

        // Assert
        finalResult.Should().Be(_randomInteger * 2 * 2, "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should not execute Synchronous binding function over Synchronous FAILED result")]
    public void Should_Not_Bind_Next_Sync_Func_On_Failure_Result()
    {
        // Act
        Result<int, TestError> boundResult = _failureResult // Synchronously failed
            .Bind(integerNumber => ExceptionThrower.AsIntResult(integerNumber));

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        using AssertionScope _ = new();

        finalResult.Should()
            .Be(TestFunctions.DefaultError.Message, "the first error should have been returned.");
    }

    [Fact(DisplayName = "Binding: Should not execute Asynchronous binding function over Synchronous FAILED result")]
    public async Task Should_Not_Bind_Next_Async_Func_On_Sync_Failure_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _failureResult // Synchronously failed
            .Bind(integerNumber => ExceptionThrower.AsIntResultAsync(integerNumber));

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        finalResult.Should()
            .Be(TestFunctions.DefaultError.Message, "the first error should have been returned.");
    }

    [Fact(DisplayName = "Binding: Should execute Synchronous binding function over Asynchronous SUCCESSFUL result")]
    public async Task Should_Bind_Next_Sync_Func_On_Async_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _successfulAsyncResult // Asynchronously Succeeded
            .Bind(integerNumber => TestFunctions.SimulateSuccess.DoubleResult(integerNumber)); // Synchronously Succeeds

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: TestFunctions.SimulateException.ThrowsAsString);

        // Assert
        finalResult.Should()
            .Be((_randomInteger * 2).ToString(), "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should execute Asynchronous binding function over Asynchronous SUCCESSFUL result")]
    public async Task Should_Bind_Next_Async_Func_On_Async_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _successfulAsyncResult // Asynchronously Succeeded
            .Bind(integerNumber => TestFunctions.SimulateSuccess.DoubleResultAsync(integerNumber)); // Asynchronously called over Asynchronous Success

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: TestFunctions.SimulateException.ThrowsAsString);

        // Assert
        finalResult.Should()
            .Be((_randomInteger * 2).ToString(), "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should not execute Asynchronous binding function over Asynchronous FAILED result")]
    public async Task Should_Not_Bind_Next_Async_Func_On_Async_Failure_Result()
    {
        // Arrange & Act
        Result<int, TestError> boundResult = await _failureResultAsync // Asynchronous Fails
            .Bind(number => ExceptionThrower.AsIntResultAsync(number)); // Asynchronously called but is not invoked

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        finalResult.Should()
            .Be(TestFunctions.DefaultError.Message, "the first error should have been returned.");
    }

    [Fact(DisplayName = "Binding: Should not execute Synchronous binding function over Asynchronous FAILED result")]
    public async Task Should_Not_Bind_Next_Sync_Func_On_Async_Failure_Result()
    {
        // Arrange & Act
        Result<int, TestError> boundResult = await _failureResultAsync // Asynchronous Fails
            .Bind(number => ExceptionThrower.AsIntResult(number)); // Synchronously called but is not invoked

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        finalResult.Should()
            .Be(TestFunctions.DefaultError.Message, "the first error should have been returned.");
    }
    
    [Fact(DisplayName = "Binding: Should propagate Synchronous error over Synchronous SUCCESSFUL result through bind operation")]
    public void Should_Bind_Error_When_Result_Is_Failure_Over_Synchronous_Successful_Result()
    {
        // Arrange & Act
        Result<int, TestError> finalResult = _successfulResult // Synchronously Succeed
            .Bind(number => TestFunctions.SimulateFailure.DoubleResult(number)) // Synchronously Fails
            .Bind(number => ExceptionThrower.AsIntResult(number)); // Synchronously called but is not invoked

        // Assert
        string resolvedMessage = finalResult.Match(
            onSuccess: _ => "Success", 
            onError: error => error.Message);

        resolvedMessage.Should().Be(TestFunctions.DefaultError.Message, "bind should preserve the first error message through chain");
    }

    [Fact(DisplayName = "Binding: Should propagate Synchronous error over Asynchronous SUCCESSFUL result through bind operation")]
    public async Task Should_Bind_Error_When_Result_Is_Failure_Over_Asynchronous_Successful_Result()
    {
        // Arrange & Act
        Result<int, TestError> finalResult = await _successfulAsyncResult // Asynchronously Succeed
            .Bind(number => TestFunctions.SimulateFailure.DoubleResult(number)) // Synchronously Fails
            .Bind(number => ExceptionThrower.AsIntResult(number)); // Synchronously called but is not invoked

        // Assert
        string resolvedMessage = finalResult.Match(
            onSuccess: _ => "Success", 
            onError: error => error.Message);

        resolvedMessage.Should().Be(TestFunctions.DefaultError.Message, "bind should preserve the first error message through chain");
    }
    
    [Fact(DisplayName = "Binding: Should propagate Asynchronous error over Synchronous SUCCESSFUL result through bind operation")]
    public async Task Should_Bind_Async_Error_When_Result_Is_Failure_Over_Synchronous_Successful_Result()
    {
        // Arrange & Act
        Result<int, TestError> finalResult = await _successfulResult // Synchronously Succeed
            .Bind(number => TestFunctions.SimulateFailure.DoubleResultAsync(number)) // Asynchronously Fails
            .Bind(number => ExceptionThrower.AsIntResult(number)); // Synchronously called but is not invoked

        // Assert
        string resolvedMessage = finalResult.Match(
            onSuccess: _ => "Success", 
            onError: error => error.Message);

        resolvedMessage.Should().Be(TestFunctions.DefaultError.Message, "bind should preserve the first error message through chain");
    }

    [Fact(DisplayName = "Binding: Should propagate Asynchronous error over Asynchronous SUCCESSFUL result through bind operation")]
    public async Task Should_Bind_Async_Error_When_Result_Is_Failure_Over_Asynchronous_Successful_Result()
    {
        // Arrange & Act
        Result<int, TestError> finalResult = await _successfulAsyncResult // Asynchronously Succeed
            .Bind(number => TestFunctions.SimulateFailure.DoubleResultAsync(number)) // Asynchronously Fails
            .Bind(number => ExceptionThrower.AsIntResult(number)); // Synchronously called but is not invoked

        // Assert
        string resolvedMessage = finalResult.Match(
            onSuccess: _ => "Success", 
            onError: error => error.Message);

        resolvedMessage.Should().Be(TestFunctions.DefaultError.Message, "bind should preserve the first error message through chain");
    }

    private static class ExceptionThrower
    {
        public static Result<int, TestError> AsIntResult(object value) =>
            Result.Success<int, TestError>(
                TestFunctions.SimulateException.ThrowsAsInt(value));

        public static Task<Result<int, TestError>> AsIntResultAsync(object value) =>
            Task.FromResult(AsIntResult(value));
    }
}