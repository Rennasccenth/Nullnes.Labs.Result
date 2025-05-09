using System.Diagnostics.Contracts;

namespace Nullnes.Labs.Result.Results;

public static class EmptyResultTaskExtensions
{
    // public static async Task<Result<TSuccess, TError>> Bind<TSuccess, TError>(
    //     this Task<Result<TSuccess, TError>> resultTask,
    //     Func<TSuccess, Task<Result<TSuccess, TError>>> onSuccess)
    // {
    //     Result<TSuccess, TError> awaitedResult = await resultTask;
    //     return await awaitedResult.Bind(onSuccess);
    // }
    //
    // public static async Task<Result<TSuccess, TError>> Bind<TSuccess, TError>(
    //     this Task<Result<TSuccess, TError>> taskResult,
    //     Func<TSuccess, Result<TSuccess, TError>> onSuccess)
    // {
    //     Result<TSuccess, TError> awaitedResult = await taskResult;
    //     return awaitedResult.Bind(onSuccess);
    // }
    //
    // public static async Task<TResponse> Match<TSuccess, TResponse, TError>(
    //     this Task<Result<TSuccess, TError>> taskResult,
    //     Func<TSuccess, TResponse> onSuccess,
    //     Func<TError, Task<TResponse>> onErrorAsync)
    // {
    //     Result<TSuccess, TError> awaitedResult = await taskResult;
    //     return await awaitedResult.Match(onSuccess, onErrorAsync);
    // }
    //
    // public static async Task<TResponse> Match<TSuccess, TResponse, TError>(
    //     this Task<Result<TSuccess, TError>> taskResult,
    //     Func<TSuccess, Task<TResponse>> onSuccessAsync,
    //     Func<TError, TResponse> onError)
    // {
    //     Result<TSuccess, TError> awaitedResult = await taskResult;
    //     return await awaitedResult.Match(onSuccessAsync, onError);
    // }
    //
    // public static async Task<TResponse> Match<TSuccess, TResponse, TError>(
    //     this Task<Result<TSuccess, TError>> taskResult,
    //     Func<TSuccess, Task<TResponse>> onSuccessAsync,
    //     Func<TError, Task<TResponse>> onErrorAsync)
    // {
    //     Result<TSuccess, TError> awaitedResult = await taskResult;
    //     return await awaitedResult.Match(onSuccessAsync, onErrorAsync);
    // }

    [Pure]
    public static async Task<Result<TOutput, TError>> Map<TError, TOutput>(
        this Task<EmptyResult<TError>> taskResult,
        Func<None, TOutput> mapping)
    {
        EmptyResult<TError> awaitedResult = await taskResult;
        return awaitedResult.Map(mapping);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Map<TError, TOutput>(
        this Task<EmptyResult<TError>> taskResult,
        Func<None, Task<TOutput>> mapping)
    {
        EmptyResult<TError> awaitedResult = await taskResult;
        return await awaitedResult.Map(mapping);
    }

    [Pure]
    public static async Task<EmptyResult<TError>> Tap<TError>(
        this Task<EmptyResult<TError>> taskResult,
        Action action)
    {
        EmptyResult<TError> awaitedResult = await taskResult;
        return awaitedResult.Tap(action);
    }
}