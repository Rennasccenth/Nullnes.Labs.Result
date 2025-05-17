using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    [Pure]
    public Result<TOutput, TError> Map<TOutput>(Func<TSuccess, TOutput> mapper) =>
        IsSuccess
            ? Result.Success<TOutput, TError>(mapper(Value)) 
            : Result.Failure<TOutput, TError>(Error);

    [Pure]
    public async Task<Result<TOutput, TError>> Map<TOutput>(Func<TSuccess, Task<TOutput>> asyncMapper)
        => IsSuccess
            ? Result.Success<TOutput, TError>(await asyncMapper(Value)) 
            : Result.Failure<TOutput, TError>(Error);
}

public static partial class AsyncResultExtensions
{
    [Pure]
    public static async Task<Result<TOutput, TError>> Map<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, Task<TOutput>> asynchronousMapping) where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Map(asynchronousMapping);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Map<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, TOutput> mapping)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.Map(mapping);
    }
}