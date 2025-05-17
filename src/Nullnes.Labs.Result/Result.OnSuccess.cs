using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    [Pure]
    public Result<TSuccess, TError> OnSuccess(Action<TSuccess> callable)
    {
        if (IsSuccess) callable(Value);
        return this;
    }

    [Pure]
    public Result<TSuccess, TError> OnSuccess(Action callable)
    {
        if (IsSuccess) callable();
        return this;
    }
}

public static partial class AsyncResultExtensions
{
    [Pure]
    public static async Task<Result<TSuccess, TError>> OnSuccess<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Action action)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.OnSuccess(action);
    }

    [Pure]
    public static async Task<Result<TSuccess, TError>> OnSuccess<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Action<TSuccess> action)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.OnSuccess(action);
    }
}