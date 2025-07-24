using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class BindingTests(ITestOutputHelper testOutputHelper)
{
    private readonly Faker _faker = new();

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
            .Bind(successValue => Result.Success<string, TestError>(successValue.ToString()));

        // Assert
        string result = finalResult.Match(
            onSuccess: successValue => successValue,
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
        TestError expectedError = new TestError($"Error occurred with while using value {value}");
        Result<int, TestError> failureResult = expectedError;

        // Act
        Result<string, TestError> finalResult = failureResult
            .Bind(successValue => Result.Success<string, TestError>(successValue.ToString()));

        // Assert
        string result = finalResult.Match(
            onSuccess: _ => "Success",
            onError: error => error.Message);

        result.Should().Be(expectedError.Message, "bind should preserve error message through chain");
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Binding")]
    [Theory(DisplayName = "Binding: Should execute synchronous binding function over synchronous successful result")]
    public void Should_Bind_Next_Sync_Func_On_Sync_Success_Result(int startingValue)
    {
        // Arrange
        Result<int, TestError> successfulResult = startingValue;
        int expectedValue = MultiplyBy10(startingValue);

        Result<int, TestError> MultiplyBy10Wrapped(int input) => MultiplyBy10(input);
        int MultiplyBy10(int input) => input * 10;

        // Act
        Result<int, TestError> boundResult = successfulResult.Bind(MultiplyBy10Wrapped);

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => 0);

        // Assert
        finalResult.Should().Be(expectedValue, "the result is a successful result, so the binding function must be executed.");
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Binding")]
    [Theory(DisplayName = "Binding: Should execute asynchronous binding function over synchronous successful result")]
    public async Task Should_Bind_Next_Async_Func_On_Sync_Success_Result(int startingValue)
    {
        // Arrange
        Result<int, TestError> successfulResult = startingValue;
        int expectedValue = MultiplyBy10(startingValue);

        Task<Result<int, TestError>> MultiplyBy10Async(int input) => Task.FromResult<Result<int, TestError>>(MultiplyBy10(input));
        int MultiplyBy10(int input) => input + 10;

        // Act
        var boundResult = await successfulResult.Bind(MultiplyBy10Async);

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => 0);

        // Assert
        finalResult.Should().Be(expectedValue, "the result is a successful result, so the binding function must be executed.");
    }

    [Trait("Category", "Binding")]
    [Fact(DisplayName = "Binding: Should not execute synchronous binding function over synchronous failed result")]
    public void Should_Not_Bind_Next_Sync_Func_On_Failure_Result()
    {
        // Arrange
        Result<int, TestError> successfulResult = new TestError("This is an Error and not a Success!");
        int expectedValueInErrorCase = _faker.Random.Int();

        // Act
        Result<int, TestError> boundResult = successfulResult.Bind(MultiplyBy10ThatThrowsException);

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => expectedValueInErrorCase);

        // Assert
        finalResult.Should()
            .Be(expectedValueInErrorCase, "the error result should have been returned.");

        static Result<int, TestError> MultiplyBy10ThatThrowsException(int input)
        {
            ThrowsException();
            return input * 10;
        }
        static void ThrowsException() => throw new InvalidOperationException($"This exception should not be thrown.");
    }

    [Trait("Category", "Binding")]
    [Fact(DisplayName = "Binding: Should not execute asynchronous binding function over synchronous failed result")]
    public async Task Should_Not_Bind_Next_Async_Func_On_Sync_Failure_Result()
    {
        // Arrange
        Result<int, TestError> successfulResult = new TestError("This is an Error and not a Success!");
        int expectedValueInErrorCase = _faker.Random.Int();

        // Act
        Result<int, TestError> boundResult = await successfulResult.Bind(MultiplyBy10AsyncThatThrowsException);

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => expectedValueInErrorCase);

        // Assert
        finalResult.Should()
            .Be(expectedValueInErrorCase, "the error result should have been returned.");

        static Task<Result<int, TestError>> MultiplyBy10AsyncThatThrowsException(int input)
        {
            ThrowsException();
            return Task.FromResult<Result<int, TestError>>(input * 10);
        }
        static void ThrowsException() => throw new InvalidOperationException($"This exception should not be thrown.");
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Binding")]
    [Theory(DisplayName = "Binding: Should execute synchronous binding function over asynchronous successful result")]
    public async Task Should_Bind_Next_Sync_Func_On_Async_Success_Result(int inputValue)
    {
        // Arrange
        Task<Result<int, TestError>> asynchronousFailedResult = Task.FromResult<Result<int, TestError>>(inputValue);

        int MultiplyBy10(int val) => val * 10;

        // Act
        Result<int, TestError> boundResult = await asynchronousFailedResult
            .Bind(x => Result.Success<int, TestError>(MultiplyBy10(x)));

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: _ => "Error");

        // Assert
        finalResult.Should().NotBe("Error", "the result is a successful result.");
        finalResult.Should().Be(MultiplyBy10(inputValue).ToString(), "the binding function should be executed over the previous result.");
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Binding")]
    [Theory(DisplayName = "Binding: Should execute asynchronous binding function over asynchronous successful result")]
    public async Task Should_Bind_Next_Async_Func_On_Async_Success_Result(int inputValue)
    {
        // Arrange
        Task<Result<int, TestError>> asynchronousSuccessfulResult = Task.FromResult<Result<int, TestError>>(inputValue);

        int MultiplyBy10(int val) => val * 10;

        // Act
        Result<int, TestError> boundResult = await asynchronousSuccessfulResult
            .Bind(x => Result.Success<int, TestError>(Task.FromResult(MultiplyBy10(x))));

        string finalResult = boundResult.Match(
            onSuccess: successResult => successResult.ToString(),
            onError: _ => "Error");

        // Assert
        finalResult.Should().NotBe("Error", "the result is a successful result.");
        finalResult.Should().Be(MultiplyBy10(inputValue).ToString(), "the binding function should be executed over the previous result.");
    }

    [Trait("Category", "Binding")]
    [Fact(DisplayName = "Binding: Should not execute asynchronous binding function over asynchronous failed result")]
    public async Task Should_Not_Bind_Next_Async_Func_On_Async_Failure_Result()
    {
        // Arrange
        Task<Result<int, TestError>> successfulResult = Task.FromResult<Result<int, TestError>>(new TestError("This is an Error and not a Success!"));
        int expectedValueInErrorCase = 0;

        Task<Result<int, TestError>> MultiplyBy10Async(int input) => Task.FromResult<Result<int, TestError>>(MultiplyBy10(input));

        // Act
        Result<int, TestError> boundResult = await successfulResult.Bind(MultiplyBy10Async);

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => expectedValueInErrorCase);

        // Assert
        finalResult.Should()
            .Be(expectedValueInErrorCase, "the error result should have been returned.");

        int MultiplyBy10(int val)
        {
            testOutputHelper.WriteLine(val.ToString());
            throw new InvalidOperationException("This exception should not be thrown.");
        }
    }

    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    [Trait("Category", "Binding")]
    [Theory(DisplayName = "Binding: Should not execute synchronous binding function over asynchronous failed result")]
    public async Task Should_Not_Bind_Next_Sync_Func_On_Async_Failure_Result(int expectedValueOnError)
    {
        // Arrange
        Task<Result<int, TestError>> asynchronousFailedResult = Task.FromResult<Result<int, TestError>>(new TestError("This is an Error and not a Success!"));

        // Act
        var boundResult = await asynchronousFailedResult.Map(x => MultiplyBy10(x));

        int finalResult = boundResult.Match(
            onSuccess: successResult => successResult,
            onError: _ => expectedValueOnError);

        // Assert
        finalResult.Should()
            .Be(expectedValueOnError, "the error result should have been returned.");
        
        int MultiplyBy10(int val)
        {
            testOutputHelper.WriteLine(val.ToString());
            throw new InvalidOperationException("This exception should not be thrown.");
        }
    }
}