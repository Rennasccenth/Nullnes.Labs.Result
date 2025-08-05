using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

[Trait("Category", "Ensure")]
public sealed class EnsureTests
{
    private readonly Faker _faker = new();
    private readonly Result<int, TestError> _successfulResult;
    private readonly Task<Result<int, TestError>> _successfulAsyncResult;
    private readonly Result<int, TestError> _failureResult;
    private readonly Task<Result<int, TestError>> _failureResultAsync;

    public EnsureTests()
    {
        _successfulResult = Result.Success<int, TestError>(_faker.Random.Int(-999_999, 999_999));
        _successfulAsyncResult = Task.FromResult(_successfulResult);

        _failureResult = Result.Failure<int, TestError>(TestFunctions.SimulateFailure.DefaultError);
        _failureResultAsync = Task.FromResult(_failureResult);
    }

    [Fact(DisplayName = "Ensure: Should not interfere Synchronous Successful flow when predicate is TRUE")]
    public void Should_Not_Interfere_Sync_Success_When_Predicate_True_ErrorFactory()
    {
        // Arrange & Act
        Result<int, TestError> ensuredResult = _successfulResult // Synchronously Succeeded  
            .Ensure(_ => true,
                errorFactory: (evaluatedValue) => new TestError($"{evaluatedValue} doesn't met the criteria"));

        // Assert
        ensuredResult
            .Should()
            .BeEquivalentTo(_successfulResult, "when the Successful value meets the criteria, the result should remains the same");
    }

    [Fact(DisplayName = "Ensure: Should not interfere Asynchronous Successful flow when predicate is TRUE")]
    public async Task Should_Not_Interfere_Async_Success_When_Predicate_True_ErrorFactory()
    {
        // Arrange & Act
        const string errorMessage = "The evaluated value doesn't met the criteria";
        string unwrappedResult = await _successfulAsyncResult // Asynchronously Succeeded  
            .Ensure(_ => true,
                errorFactory: _ => new TestError(errorMessage))
            .Match(
                successValue => $"{successValue} meets the criteria",
                error => error.Message);

        // Assert
        unwrappedResult
            .Should()
            .NotBe(errorMessage, "when the Successful value meets the criteria, the result should remains the same");
    }

    [Fact(DisplayName = "Ensure: Should return Failure when predicate is FALSE over an Synchronous Successful flow")]
    public void Should_Return_Failure_When_Predicate_Is_False_Over_Sync_Success()
    {
        // Arrange & Act
        const string errorMessage = "The evaluated value doesn't met the criteria";
        
        Result<int, TestError> ensuredResult = _successfulResult // Synchronously Succeeded
            .Ensure(_ => false,
                errorFactory: _ => new TestError(errorMessage));

        // Assert
        ensuredResult
            .Match(
                onSuccess: _ => true,
                onError: _ => false)
            .Should()
            .BeFalse("when the Successful value doesn't meet the criteria, the result should be turned into a failure");

        ensuredResult
            .Match(
                onSuccess: _ => Guid.NewGuid().ToString(),
                onError: error => error.Message)
            .Should()
            .Be(errorMessage, "the error should be created by the error factory parameter");
    }

    [Fact(DisplayName = "Ensure: Should return Failure when predicate is FALSE over an Asynchronous Successful flow")]
    public async Task Should_Return_Failure_When_Predicate_Is_False_Over_Async_Success()
    {
        // Arrange & Act
        const string errorMessage = "The evaluated value doesn't met the criteria";
        
        Result<int, TestError> ensuredResult = await _successfulAsyncResult // Synchronously Succeeded
            .Ensure(_ => false,
                errorFactory: _ => new TestError(errorMessage));
        
        // Assert
        ensuredResult
            .Match(
                onSuccess: _ => true,
                onError: _ => false)
            .Should()
            .BeFalse("when the Successful value doesn't meet the criteria, the result should be turned into a failure");

        ensuredResult
            .Match(
                onSuccess: _ => Guid.NewGuid().ToString(),
                onError: error => error.Message)
            .Should()
            .Be(errorMessage, "the error should be created by the error factory parameter");
    }

    [Fact(DisplayName = "Ensure: Should propagate existing Failure without interfere over Synchronous failure")]
    public void Should_Propagate_Existing_Failure_All_Overloads_On_SynchronousFailure()
    {
        // Arrange & Act
        const string errorMessage = "The evaluated value doesn't met the criteria";

        Result<int, TestError> ensuredFirst = _failureResult.Ensure(_ => true, errorFactory: _ => new TestError(errorMessage));
        Result<int, TestError> ensuredSecond = _failureResult.Ensure(_ => true, errorInstance: new TestError(errorMessage));

        // Assert
        ensuredFirst.Should().BeEquivalentTo(_failureResult, errorMessage);
        ensuredSecond.Should().BeEquivalentTo(_failureResult, errorMessage);
    }
    
    [Fact(DisplayName = "Ensure: Should propagate existing Failure without interfere over Asynchronous failure")]
    public async Task Should_Propagate_Existing_Failure_All_Overloads_On_AsynchronousFailure()
    {
        // Arrange & Act
        const string errorMessage = "The evaluated value doesn't met the criteria";

        Result<int, TestError> ensuredFirst = await _failureResultAsync.Ensure(_ => true, errorFactory: _ => new TestError(errorMessage));
        Result<int, TestError> ensuredSecond = await _failureResultAsync.Ensure(_ => true, errorInstance: new TestError(errorMessage));

        // Assert
        ensuredFirst.Should().BeEquivalentTo(_failureResult, errorMessage);
        ensuredSecond.Should().BeEquivalentTo(_failureResult, errorMessage);
    }
}
