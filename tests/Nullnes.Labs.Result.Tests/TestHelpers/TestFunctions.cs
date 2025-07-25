using System.Runtime.CompilerServices;

namespace Nullnes.Labs.Result.Tests.TestHelpers;

/// <summary>
/// Common functions to be used while simulating test scenarios 
/// </summary>
internal static class TestFunctions
{
    internal static int Double(int input) => input * 2; 
    internal static Task<int> DoubleAsync(int input) => Task.FromResult(Double(input));
    internal static void ThrowsDefaultException(Exception? exception = null, [CallerMemberName] string callerMethodName = "") => throw exception ?? throw GetDefaultException(callerMethodName);
    internal static NotSupportedException GetDefaultException([CallerMemberName] string callerMethodName = "") => new($"{callerMethodName} threw an exception");

    /// <summary>
    /// Provides functions that encapsulates an successful result
    /// </summary>
    internal static class SimulateSuccess
    {
        internal static Result<int, TestError> DoubleResult(int input) => Double(input);
        internal static Task<Result<int, TestError>> DoubleResultAsync(int input) => Task.FromResult(Result.Success<int, TestError>(Double(input)));
        internal static Result<string, TestError> ToStringResult(int input) => Result.Success<string, TestError>(input.ToString());
        internal static Task<Result<string, TestError>> ToStringResultAsync(int input) => Task.FromResult(Result.Success<string, TestError>(input.ToString()));
    }

    /// <summary>
    /// Provides functions that encapsulates an failure result, returning a <see cref="DefaultError"/> instance.
    /// </summary>
    internal static class SimulateFailure
    {
        /// <summary>
        /// A simple <see cref="TestError"/> instance used to simulate tests scenarios
        /// </summary>
        internal static readonly TestError DefaultError = new(message: "An error has occurred");
        
        internal static Result<int, TestError> DoubleResult(int _) => DefaultError;
        internal static async Task<Result<int, TestError>> DoubleResultAsync(int _) => Result.Failure<int, TestError>(await Task.FromResult(DefaultError));
        internal static Result<string, TestError> ToStringResult(int _) => Result.Failure<string, TestError>(DefaultError);
        internal static Task<Result<string, TestError>> ToStringResultAsync(int _) => Task.FromResult(Result.Failure<string, TestError>(DefaultError));
    }
}

