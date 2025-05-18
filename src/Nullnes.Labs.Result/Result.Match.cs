using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    [Pure]
    public TOutput Match<TOutput>(
        Func<TSuccess, TOutput> onSuccess,
        Func<TError, TOutput> onError) =>
        IsSuccess
            ? onSuccess(Value) 
            : onError(Error);

    [Pure]
    public Task<TOutput> Match<TOutput>(
        Func<TSuccess, Task<TOutput>> onSuccessAsync,
        Func<TError, TOutput> onError) =>
        IsSuccess 
            ? onSuccessAsync(Value) 
            : Task.FromResult(onError(Error));

    [Pure]
    public Task<TOutput> Match<TOutput>(
        Func<TSuccess, TOutput> onSuccess,
        Func<TError, Task<TOutput>> onErrorAsync) =>
        IsSuccess
            ? Task.FromResult(onSuccess(Value)) 
            : onErrorAsync(Error);

    [Pure]
    public Task<TOutput> Match<TOutput>(
        Func<TSuccess, Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onErrorAsync) =>
        IsSuccess
            ? onSuccessAsync(Value) 
            : onErrorAsync(Error);

    [Pure]
    public TOutput Match<TOutput>(
        Func<TOutput> onSuccess,
        Func<TError, TOutput> onError) =>
        IsSuccess
            ? onSuccess() 
            : onError(Error);

    [Pure]
    public Task<TOutput> Match<TOutput>(
        Func<Task<TOutput>> onSuccessAsync,
        Func<TError, TOutput> onError) =>
        IsSuccess 
            ? onSuccessAsync() 
            : Task.FromResult(onError(Error));

    [Pure]
    public Task<TOutput> Match<TOutput>(
        Func<TOutput> onSuccess,
        Func<TError, Task<TOutput>> onErrorAsync) =>
        IsSuccess
            ? Task.FromResult(onSuccess()) 
            : onErrorAsync(Error);

    [Pure]
    public Task<TOutput> Match<TOutput>(
        Func<Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onErrorAsync) =>
        IsSuccess
            ? onSuccessAsync() 
            : onErrorAsync(Error);
}

public static partial class AsyncResultExtensions
{
    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, TOutput> onSuccess,
        Func<TError, TOutput> onError)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.Match(onSuccess, onError);
    }

    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, TOutput> onSuccess,
        Func<TError, Task<TOutput>> onErrorAsync)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Match(onSuccess, onErrorAsync);
    }

    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, Task<TOutput>> onSuccessAsync,
        Func<TError, TOutput> onError)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Match(onSuccessAsync, onError);
    }

    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onErrorAsync)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Match(onSuccessAsync, onErrorAsync);
    }

    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TOutput> onSuccess,
        Func<TError, TOutput> onError)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.Match(onSuccess, onError);
    }

    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<Task<TOutput>> onSuccessAsync,
        Func<TError, TOutput> onError)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Match(onSuccessAsync, onError);
    }

    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TOutput> onSuccess,
        Func<TError, Task<TOutput>> onErrorAsync)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Match(onSuccess, onErrorAsync);
    }

    [Pure]
    public static async Task<TOutput> Match<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onErrorAsync)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Match(onSuccessAsync, onErrorAsync);
    }
}