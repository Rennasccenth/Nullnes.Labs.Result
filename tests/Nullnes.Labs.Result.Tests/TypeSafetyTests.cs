using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class TypeSafetyTests
{
    [Fact(DisplayName = "TypeSafety: A Successful Result must be created given a non Error Type value")]
    [Trait("Category", "TypeSafety")]
    public void Should_Implicitly_Convert_Value_To_Result()
    {
        // Arrange
        int value = 42;

        // Act
        Result<int, TestError> result = value;

        // Assert
        bool resultValue = result
            .Match(
                onSuccess: _ => true,
                onError: _ => false);

        resultValue.Should().BeTrue(because: "value should be implicitly converted to successful result");
    }

    [Fact(DisplayName = "TypeSafety: A Failure Result must be created given a Error Type value")]
    [Trait("Category", "TypeSafety")]
    public void Should_Implicitly_Convert_Error_To_Result()
    {
        // Arrange
        TestError error = new("Test error");

        // Act
        Result<dynamic, TestError> result = error;

        string resultValue = result.Match(
            onSuccess: obj => obj.ToString(),
            onError: err => err.Message);

        // Assert
        resultValue.Should().Be(error.Message, because: "error should be implicitly converted to failed result");
    }
}