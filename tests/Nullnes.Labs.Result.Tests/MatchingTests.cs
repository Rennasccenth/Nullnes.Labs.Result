using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class MatchingTests
{
    private readonly Result<int, TestError> _successfulResult = Result.Success<int, TestError>(new Faker().Random.Int(-999, 999));
    private readonly Result<int, TestError> _failureResult = Result.Failure<int, TestError>(TestFunctions.DefaultError);
    
    private readonly Task<Result<int, TestError>> _failureResultAsync;
    private readonly Task<Result<int, TestError>> _successfulResultAsync;
    public MatchingTests()
    {
        _failureResultAsync = Task.FromResult(_failureResult);
        _successfulResultAsync = Task.FromResult(_successfulResult);
    }

    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Sync Success function for Sync SUCCESSFUL Result")]
    public void Should_Match_Sync_Success_When_Result_Is_Sync_Success()
    {
        // Arrange & Act
        var act = () => _successfulResult
            .Match(
                onSuccess: successValue => successValue.ToString(),
                onError: error => TestFunctions.SimulateException.ThrowsAsString(error));

        // Assert
        act.Should()
            .NotThrow(because: "the onSuccess function should be executed for sync successful Result");
    }

    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Sync Success function for Async SUCCESSFUL Result")]
    public async Task Should_Match_Sync_Success_When_Result_Is_Async_Success()
    {
        // Arrange & Act
        var act = async () => await _successfulResultAsync
            .Match(
                onSuccess: successValue => successValue.ToString(),
                onError: error => TestFunctions.SimulateException.ThrowsAsString(error));

        // Assert
        await act.Should()
            .NotThrowAsync(because: "the sync onSuccess function should be executed for sync successful Result");
    }

    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Async Success function for Async SUCCESSFUL Result")]
    public async Task Should_Match_Async_Success_When_Result_Is_Async_Success()
    {
        // Arrange & Act
        var act = async () => await _successfulResultAsync
            .Match(
                onSuccess: successValue => Task.FromResult(successValue.ToString()),
                onError: error => TestFunctions.SimulateException.ThrowsAsStringAsync(error));

        // Assert
        await act.Should()
            .NotThrowAsync(because: "the async onSuccess function should be executed for async successful Result");
    }

    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Async Success function for Sync SUCCESSFUL Result")]
    public async Task Should_Match_Async_Success_When_Result_Is_Sync_Success()
    {
        // Arrange & Act
        var act = async () => await _successfulResult
            .Match(
                onSuccess: successValue => Task.FromResult(successValue.ToString()),
                onError: error => TestFunctions.SimulateException.ThrowsAsStringAsync(error));

        // Assert
        await act.Should()
            .NotThrowAsync(because: "the async onSuccess function should be executed for sync successful Result");
    }

    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Sync Error function for Sync FAILED Result ")]
    public void Should_Match_Sync_Error_When_Result_Is_Sync_Failure()
    {
        // Arrange & Act
        var act = () => _failureResult
            .Match(
                onSuccess: successValue => TestFunctions.SimulateException.ThrowsAsString(successValue),
                onError: error => error.Message);

        // Assert
        act.Should()
            .NotThrow(because: "the sync onError function should be executed for sync failure result");
    }

    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Async Error function for Sync FAILED Result ")]
    public async Task Should_Match_Async_Error_When_Result_Is_Sync_Failure()
    {
        // Arrange & Act
        var act = async () => await _failureResult
            .Match(
                onSuccess: successValue => TestFunctions.SimulateException.ThrowsAsStringAsync(successValue),
                onError: error => Task.FromResult(error.Message));

        // Assert
        await act.Should()
            .NotThrowAsync(because: "the async onError function should be executed for sync failure result");
    }

    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Sync Error function for Async FAILED Result ")]
    public async Task Should_Match_Sync_Error_When_Result_Is_Async_Failure()
    {
        // Arrange & Act
        var act = async () => await _failureResultAsync
            .Match(
                onSuccess: successValue => TestFunctions.SimulateException.ThrowsAsString(successValue),
                onError: error => error.Message);

        // Assert
        await act.Should()
            .NotThrowAsync(because: "the sync onError function should be executed for async failure result");
    }
    
    [Trait("Category", "Matching")]
    [Fact(DisplayName = "Matching: Should execute Async Error function for Async FAILED Result ")]
    public async Task Should_Match_Async_Error_When_Result_Is_Async_Failure()
    {
        // Arrange & Act
        var act = async () => await _failureResultAsync
            .Match(
                onSuccess: successValue => TestFunctions.SimulateException.ThrowsAsStringAsync(successValue),
                onError: error => Task.FromResult(error.Message));

        // Assert
        await act.Should()
            .NotThrowAsync(because: "the async onError function should be executed for async failure result");
    }
}