using System.Diagnostics.Contracts;

namespace Nullnes.Labs.Result.Results;

public static class ResultTaskExtensions
{
    [Pure]
    public static async Task<Result<TSuccess, TError>> Bind<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, Task<Result<TSuccess, TError>>> onSuccess)
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Bind(onSuccess);
    }

    [Pure]
    public static async Task<Result<TSuccess, TError>> Bind<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, Result<TSuccess, TError>> onSuccess)
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return awaitedResult.Bind(onSuccess);
    }

    [Pure]
    public static async Task<TResponse> Match<TSuccess, TResponse, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, TResponse> onSuccess,
        Func<TError, Task<TResponse>> onErrorAsync)
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return await awaitedResult.Match(onSuccess, onErrorAsync);
    }

    [Pure]
    public static async Task<TResponse> Match<TSuccess, TResponse, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, Task<TResponse>> onSuccessAsync,
        Func<TError, TResponse> onError)
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return await awaitedResult.Match(onSuccessAsync, onError);
    }

    [Pure]
    public static async Task<TResponse> Match<TSuccess, TResponse, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, Task<TResponse>> onSuccessAsync,
        Func<TError, Task<TResponse>> onErrorAsync)
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return await awaitedResult.Match(onSuccessAsync, onErrorAsync);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Map<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, Task<TOutput>> asynchronousMapping)
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return await awaitedResult.Map(asynchronousMapping);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Map<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, TOutput> mapping)
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return awaitedResult.Map(mapping);
    }

    [Pure]
    public static async Task<Result<TSuccess, TError>> Tap<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Action<TSuccess> action)
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return awaitedResult.Tap(action);
    }
}