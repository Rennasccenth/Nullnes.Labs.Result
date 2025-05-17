using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    [Pure]
    public Result<TSuccess, TError> OnFailure(Action<TError> callable)
    {
        if (IsFailure) callable(Error);
        return this;
    }

    [Pure]
    public Result<TSuccess, TError> OnFailure(Action callable)
    {
        if (IsFailure) callable();
        return this;
    }
}

public static partial class AsyncResultExtensions
{
    [Pure]
    public static async Task<Result<TSuccess, TError>> OnFailure<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Action action)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.OnFailure(action);
    }

    [Pure]
    public static async Task<Result<TSuccess, TError>> OnFailure<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Action<TError> action)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.OnFailure(action);
    }
}