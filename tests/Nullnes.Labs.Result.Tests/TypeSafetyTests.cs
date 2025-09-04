using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class TypeSafetyTests
{
    [Fact(DisplayName = "TypeSafety: A Successful Result must be created given a non Error Type value")]
    [Trait("Category", "TypeSafety")]
    public void Should_Implicitly_Convert_Value_To_Result()
    {
        // Arrange
        const int value = 42;

        // Act
        Result<int, TestError> result = value;

        // Assert
        var act = () => result
            .Match(
                onSuccess: number => number,
                onError: TestFunctions.SimulateException.ThrowsAsInt);

        act.Should().NotThrow(because: "the value should be converted to a non Error Type value");
    }

    [Fact(DisplayName = "TypeSafety: A Failure Result must be created given a Error Type value")]
    [Trait("Category", "TypeSafety")]
    public void Should_Implicitly_Convert_Error_To_Result()
    {
        // Arrange
        TestError error = new("Test error");

        // Act
        Result<dynamic, TestError> result = error;

        var act = () => result
            .Match(
                onSuccess: TestFunctions.SimulateException.ThrowsAsString,
                onError: err => err.Message);

        // Assert
        act.Should().NotThrow(because: "error should be implicitly converted to failed result");
    }
}