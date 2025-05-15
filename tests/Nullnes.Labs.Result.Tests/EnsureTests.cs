using FluentAssertions;
using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

[Trait("Category", "Ensure")]
public sealed class EnsureTests
{
    [Theory(DisplayName = "Ensure: Should not affect Success when predicate is true (errorFactory overload)")]
    [InlineData(1)]
    [InlineData(100)]
    public void Should_Not_Affect_Success_When_Predicate_True_ErrorFactory(int value)
    {
        // Arrange
        Result<int, TestError> successfulResult = value;

        // Act
        Result<int, TestError> ensuredResult = successfulResult.Ensure(x => x > 0,
            (evaluatedValue) => new TestError($"{evaluatedValue} doesn't met the criteria"));

        // Assert
        ensuredResult
            .Should()
            .BeEquivalentTo(successfulResult, "when the successful value meets the criteria, the result should remains the same");
    }

    [Theory(DisplayName = "Ensure: Should return Failure when predicate is false (errorFactory overload)")]
    [InlineData(-1)]
    [InlineData(0)]
    public void Should_Return_Failure_When_Predicate_False_ErrorFactory(int value)
    {
        // Arrange
        Result<int, TestError> successfulResult = value;
        
        // Act
        Result<int, TestError> ensuredResult = successfulResult
            .Ensure(x => x > 0,
                (evaluatedValue) => new TestError($"{evaluatedValue} doesn't met the criteria"));

        // Assert
        ensuredResult.Match(onSuccess: _ => false, onError: _ => true)
            .Should()
            .BeTrue("when the successful value doesn't meet the criteria, the result should be a failure");

        ensuredResult.Match(onSuccess: _ => "This should not be called", onError: error => error.Message)
            .Should()
            .Be($"{value} doesn't met the criteria", "the error should be created by the error factory parameter");
    }

    [Theory(DisplayName = "Ensure: Should not affect Success when predicate is true (errorInstance overload)")]
    [InlineData(1)]
    [InlineData(100)]
    public void Should_Not_Affect_Success_When_Predicate_True_ErrorInstance(int value)
    {
        // Arrange
        Result<int, TestError> successfulResult = value;
        
        // Act
        Result<int, TestError> ensuredResult = successfulResult.Ensure(x => x > 0, new TestError("Fail"));
        
        // Assert
        ensuredResult.Should().BeEquivalentTo(successfulResult, 
            "when the successful value meets the criteria, the result should remains the same");
    }

    [Theory(DisplayName = "Ensure: Should return Failure when predicate is false (errorInstance overload)")]
    [InlineData(-1)]
    [InlineData(0)]
    public void Should_Return_Failure_When_Predicate_Is_False_On_ErrorInstance(int value)
    {
        // Arrange
        const string errorMessage = "Predicate has failed";
        Result<int, TestError> successfulResult = value;

        // Act
        Result<int, TestError> ensuredResult = successfulResult.Ensure(x => x > 0, new TestError(errorMessage));

        // Assert
        ensuredResult.Match(onSuccess: _ => false, onError: _ => true)
            .Should()
            .BeTrue("when the successful value doesn't meet the criteria, the result should be a failure");

        ensuredResult.Match(onSuccess: _ => false, onError: err => err.Message == errorMessage)
            .Should()
            .BeTrue("the error should be created by the error instance parameter");
    }

    [Fact(DisplayName = "Ensure: Should propagate existing Failure without modification (all overloads)")]
    public void Should_Propagate_Existing_Failure_All_Overloads()
    {
        // Arrange
        Result<int, TestError> failedResult = new TestError("This is the ORIGINAL failure.");

        // Act
        Result<int, TestError> ensuredFirst = failedResult.Ensure(x => x > 0, _ => new TestError("This is a Different failure"));
        Result<int, TestError> ensuredSecond = failedResult.Ensure(x => x > 0, new TestError("This also is a Different failure"));

        // Assert
        ensuredFirst.Should().BeEquivalentTo(failedResult, "the existing failure should not be modified");
        ensuredSecond.Should().BeEquivalentTo(failedResult, "the existing failure should not be modified");
    }
}
