using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public static class ResultTaskExtensions
{
    #region Bind
    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, Task<Result<TOutput, TError>>> onSuccess) 
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Bind(onSuccess);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, Result<TOutput, TError>> onSuccess)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return awaitedResult.Bind(onSuccess);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<Task<Result<TOutput, TError>>> asyncBinder)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return await awaitedResult.Bind(asyncBinder);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TOutput, TError>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<Result<TOutput, TError>> binder)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return awaitedResult.Bind(binder);
    }
    #endregion

    # region Match
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
    #endregion

    # region Map
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
    # endregion

    # region On Success
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
    # endregion
    
    # region On Failure
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
    # endregion
    
    # region Ensure

    [Pure]
    public static async Task<Result<TSuccess, TError>> Ensure<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, bool> predicate,
        Func<TSuccess, TError> errorFactory) where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.Ensure(predicate, errorFactory);
    }

    [Pure]
    public static async Task<Result<TSuccess, TError>> Ensure<TSuccess, TError>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, bool> predicate,
        TError errorInstance) where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return awaitedResult.Ensure(predicate, errorInstance);
    }

    # endregion
}