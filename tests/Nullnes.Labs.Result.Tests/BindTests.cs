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

        _failureResult = Result.Failure<int, TestError>(TestFunctions.SimulateFailure.DefaultError);
        _failureResultAsync = Task.FromResult(_failureResult);
    }

    [Fact(DisplayName = "Binding: Should propagate error through bind operation")]
    public void Should_Bind_Error_When_Result_Is_Failure()
    {
        // Arrange
        TestError secondError = new("This error should never happen, because bind should preserve the previous error.");

        // Act
        Result<int, TestError> finalResult = TestFunctions.SimulateSuccess.DoubleResult(_randomInteger) // Synchronously Succeed
            .Bind(doubledValue => TestFunctions.SimulateFailure.DoubleResult(doubledValue)) // Synchronously Fails
            .Bind(quadrupledValue => secondError); // Synchronously called but is not invoked

        // Assert
        string resolvedMessage = finalResult.Match(
            onSuccess: _ => "Success", 
            onError: error => error.Message);

        resolvedMessage.Should().Be(TestFunctions.SimulateFailure.DefaultError.Message, "bind should preserve the first error message through chain");
    }

    [Fact(DisplayName = "Binding: Should execute synchronous binding function over synchronous successful result")]
    public void Should_Bind_Next_Sync_Func_On_Sync_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = _successfulResult // Synchronously Succeeded
            .Bind(randomInteger => TestFunctions.SimulateSuccess.DoubleResult(randomInteger)) // Synchronously Succeeded
            .Bind(doubledValue => TestFunctions.SimulateSuccess.DoubleResult(doubledValue)); // Synchronously Bound to a success

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => TestFunctions.Double(_randomInteger));

        // Assert
        finalResult.Should().Be(_randomInteger * 2 * 2, "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should execute asynchronous binding function over synchronous successful result")]
    public async Task Should_Bind_Next_Async_Func_On_Sync_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _successfulResult
            .Bind(randomInteger => TestFunctions.SimulateSuccess.DoubleResult(randomInteger)) // Synchronously Succeeds
            .Bind(doubledValue => TestFunctions.SimulateSuccess.DoubleResultAsync(doubledValue)); // Asynchronously Bound to a Synchronous Success

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => 0);

        // Assert
        finalResult.Should().Be(_randomInteger * 2 * 2, "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should not execute synchronous binding function over synchronous failed result")]
    public void Should_Not_Bind_Next_Sync_Func_On_Failure_Result()
    {
        // Act
        Result<int, TestError> boundResult = _failureResult // Synchronously Failed
            .Bind(integerNumber => 
            {
                TestFunctions.ThrowsDefaultException(); // If the previously chained bind works as intended,
                                                        // this should never be thrown 
                return TestFunctions.SimulateSuccess.DoubleResult(integerNumber); 
            });

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        using AssertionScope _ = new();

        finalResult.Should()
            .Be(TestFunctions.SimulateFailure.DefaultError.Message, "the first error should have been returned.");
    }

    [Fact(DisplayName = "Binding: Should not execute asynchronous binding function over synchronous failed result")]
    public async Task Should_Not_Bind_Next_Async_Func_On_Sync_Failure_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _failureResult // Synchronously Failed
            .Bind(integerNumber =>
            {
                TestFunctions.ThrowsDefaultException(); // If the previously chained bind works as intended,
                                                        // this should never be thrown 
                return TestFunctions.SimulateSuccess.DoubleResultAsync(integerNumber);
            });

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        finalResult.Should()
            .Be(TestFunctions.SimulateFailure.DefaultError.Message, "the first error should have been returned.");
    }

    [Fact(DisplayName = "Binding: Should execute synchronous binding function over asynchronous successful result")]
    public async Task Should_Bind_Next_Sync_Func_On_Async_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _successfulAsyncResult // Asynchronously Succeeded
            .Bind(integerNumber => TestFunctions.SimulateSuccess.DoubleResult(integerNumber)); // Synchronously Succeeds

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: _ => throw TestFunctions.GetDefaultException());

        // Assert
        finalResult.Should()
            .Be((_randomInteger * 2).ToString(), "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should execute asynchronous binding function over asynchronous successful result")]
    public async Task Should_Bind_Next_Async_Func_On_Async_Success_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _successfulAsyncResult // Asynchronously Succeeded
            .Bind(integerNumber => TestFunctions.SimulateSuccess.DoubleResultAsync(integerNumber)); // Asynchronously called over Asynchronous Success

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: _ => throw TestFunctions.GetDefaultException());

        // Assert
        finalResult.Should()
            .Be((_randomInteger * 2).ToString(), "the binding function should be executed over the previous result.");
    }

    [Fact(DisplayName = "Binding: Should not execute asynchronous binding function over asynchronous failed result")]
    public async Task Should_Not_Bind_Next_Async_Func_On_Async_Failure_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _failureResultAsync // Asynchronous Fails
            .Bind(integerNumber =>
            {
                TestFunctions.ThrowsDefaultException(); // If the previously chained bind works as intended,
                                                        // this should never be thrown 
                return TestFunctions.SimulateSuccess.DoubleResultAsync(integerNumber);
            });

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        finalResult.Should()
            .Be(TestFunctions.SimulateFailure.DefaultError.Message, "the first error should have been returned.");
    }

    [Fact(DisplayName = "Binding: Should not execute synchronous binding function over asynchronous failed result")]
    public async Task Should_Not_Bind_Next_Sync_Func_On_Async_Failure_Result()
    {
        // Act
        Result<int, TestError> boundResult = await _failureResultAsync // Asynchronous Fails
            .Bind(integerNumber =>
            {
                TestFunctions.ThrowsDefaultException(); // If the previously chained bind works as intended,
                                                        // this should never be thrown 
                return TestFunctions.SimulateSuccess.DoubleResult(integerNumber);
            });

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: error => error.Message);

        // Assert
        finalResult.Should()
            .Be(TestFunctions.SimulateFailure.DefaultError.Message, "the first error should have been returned.");
    }
}