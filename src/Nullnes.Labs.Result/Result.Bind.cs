using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    [Pure]
    public Result<TSuccess, TError> Bind(Func<TSuccess, Result<TSuccess, TError>> binder) 
        => IsSuccess ? binder(Value) : this;

    [Pure]
    public Result<TOutput, TError> Bind<TOutput>(Func<TSuccess, Result<TOutput, TError>> binder)
        => IsSuccess ? binder(Value) : Result.Failure<TOutput, TError>(Error);

    [Pure]
    public Result<TSuccess, TError> Bind(Func<Result<TSuccess, TError>> binder) => 
        IsSuccess ? binder() : this; 

    [Pure]
    public Result<TOutput, TError> Bind<TOutput>(Func<Result<TOutput, TError>> binder)
        => IsSuccess ? binder() : Result.Failure<TOutput, TError>(Error); 

    [Pure]
    public Task<Result<TSuccess, TError>> Bind(Func<TSuccess, Task<Result<TSuccess, TError>>> asyncBinder)
        => IsSuccess ? asyncBinder(Value) : Task.FromResult(this);

    [Pure]
    public Task<Result<TOutput, TError>> Bind<TOutput>(Func<TSuccess, Task<Result<TOutput, TError>>> asyncBinder)
        => IsSuccess ? asyncBinder(Value) : Task.FromResult(Result.Failure<TOutput, TError>(Error));

    [Pure]
    public Task<Result<TSuccess, TError>> Bind(Func<Task<Result<TSuccess, TError>>> asyncBinder)
        => IsSuccess ? asyncBinder() : Task.FromResult(this);
}

public static class ResultExtensions
{
    [Pure]
    public static Task<Result<TOutput, TError>> Bind<TSuccess, TOutput, TError>(
        this Result<TSuccess, TError> result,
        Func<Task<Result<TOutput, TError>>> asyncBinder)
        where TError : class, IError
        => result.IsSuccess
            ? asyncBinder()
            : Task.FromResult(Result.Failure<TOutput, TError>(result.Error));
}

public static partial class AsyncResultExtensions
{
    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> resultTask,
        Func<TSuccess, Task<Result<TOutput, TError>>> onSuccess) 
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await resultTask;
        return await awaitedResult.Bind(onSuccess);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<TSuccess, Result<TOutput, TError>> onSuccess)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return awaitedResult.Bind(onSuccess);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<Result<TOutput, TError>> binder)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return awaitedResult.Bind(binder);
    }

    [Pure]
    public static async Task<Result<TOutput, TError>> Bind<TSuccess, TError, TOutput>(
        this Task<Result<TSuccess, TError>> taskResult,
        Func<Task<Result<TOutput, TError>>> asyncBinder)
        where TError : class, IError
    {
        Result<TSuccess, TError> awaitedResult = await taskResult;
        return await awaitedResult.Bind(asyncBinder);
    }
}