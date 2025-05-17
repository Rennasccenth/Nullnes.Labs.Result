using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    [Pure]
    public Result<TSuccess, TNewError> MapError<TNewError>(Func<TError, TNewError> errorMapper) where TNewError : class, IError
    {
        return IsSuccess 
            ? Result.Success<TSuccess, TNewError>(Value) 
            : Result.Failure<TSuccess, TNewError>(errorMapper(Error));
    }
}

public static partial class AsyncResultExtensions
{
    [Pure]
    public static async Task<Result<TSuccess, TNewError>> MapError<TSuccess, TError, TNewError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TError, TNewError> errorMapper) where TNewError : class, IError where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.MapError(errorMapper);
    }
}