using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result.Tests.TestHelpers;

/// <summary>
/// Simple class used to represent errors.
/// Since there's no constraint about the TError type, in Result monad, you can create any type to represent yours.  
/// </summary>
internal sealed class TestError(string message) : IError
{
    public string Message { get; } = message;
}
