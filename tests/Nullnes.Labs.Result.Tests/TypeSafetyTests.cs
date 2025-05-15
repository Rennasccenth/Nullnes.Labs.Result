using System.Globalization;
using FluentAssertions;
using Nullnes.Labs.Result.Abstractions;
using Nullnes.Labs.Result.Tests.TestHelpers;

namespace Nullnes.Labs.Result.Tests;

public sealed class TypeSafetyTests
{
    [Fact]
    [Trait("Category", "TypeSafety")]
    public void Should_Implicitly_Convert_Value_To_Result()
    {
        // Arrange
        int value = 42;

        // Act
        Result<int, TestError> result = value;

        // Assert
        bool isSuccess = false;
        string resultValue = result.Match(
            onSuccess: v => { isSuccess = true; return v.ToString(); },
            onError: error => error.Message);

        isSuccess.Should().BeTrue("value should be implicitly converted to successful result");
        resultValue.Should().Be(value.ToString());
    }

    [Fact]
    [Trait("Category", "TypeSafety")]
    public void Should_Implicitly_Convert_Error_To_Result()
    {
        // Arrange
        var error = new TestError("Test error");

        // Act
        Result<int, TestError> result = error;

        // Assert
        bool isSuccess = true;
        string resultValue = result.Match(
            onSuccess: v => { isSuccess = true; return v.ToString(); },
            onError: e => { isSuccess = false; return e.Message; });

        isSuccess.Should().BeFalse("error should be implicitly converted to failed result");
        resultValue.Should().Be(error.Message);
    }

    [Fact]
    [Trait("Category", "TypeSafety")]
    public void Should_Preserve_Type_Safety_Through_Map()
    {
        // Arrange
        Result<int, TestError> result = 42;

        // Act
        Result<string, TestError> mappedResult = result.Map(x => x.ToString());

        // Assert
        string resultValue = mappedResult.Match(
            onSuccess: value => value,
            onError: error => error.Message);

        resultValue.Should().Be("42", "mapping should transform the value");
    }

    [Fact]
    [Trait("Category", "TypeSafety")]
    public void Should_Preserve_Type_Safety_Through_Bind()
    {
        // Arrange
        Result<int, TestError> result = 42;

        // Act
        Result<string, TestError> boundResult = result.Map(x => x.ToString());

        // Assert
        string resultValue = boundResult.Match(
            onSuccess: value => value,
            onError: error => error.Message);

        resultValue.Should().Be("42", "binding should transform the value");
    }

    [Fact]
    [Trait("Category", "TypeSafety")]
    public void Should_Preserve_Error_Type_Through_Operations()
    {
        // Arrange
        const string errorMessage = "Error occurred";

        TestError error = new(errorMessage);
        Result<int, TestError> result = error;

        // Act
        Result<string, TestError> transformedResult = result.Map(x => x.ToString(CultureInfo.InvariantCulture));

        // Assert
        string resultValue = transformedResult.Match(
            onSuccess: _ => "Success",
            onError: err => err.Message);

        resultValue.Should()
            .Be(error.Message, "error state should be preserved through operations.");
    }

    [Fact]
    [Trait("Category", "TypeSafety")]
    public void Should_Handle_None_Type_Safety()
    {
        // Arrange
        Result<None, TestError> none = None.Value;
        const int mappingValue = 42;

        // Act
        Result<int, TestError> result = none.Map(_ => mappingValue);

        // Assert
        string resultValue = result.Match(
            onSuccess: value => value.ToString(CultureInfo.InvariantCulture),
            onError: error => error.Message);

        resultValue.Should().Be(mappingValue.ToString(CultureInfo.InvariantCulture), "None should be transformed to success");
    }

    [Fact]
    [Trait("Category", "TypeSafety")]
    public void Should_Handle_Empty_Result_Type_Safety()
    {
        // Arrange
        Result<None, TestError> emptyResult = None.Value;
        const int mappingValue = 42;

        // Act
        Result<int, TestError> result = emptyResult.Map(_ => mappingValue);

        // Assert
        string resultValue = result.Match(
            onSuccess: value => value.ToString(CultureInfo.InvariantCulture),
            onError: error => error.Message);

        resultValue.Should()
            .Be(mappingValue.ToString(CultureInfo.InvariantCulture), "empty result should be transformed to success");
    }
}