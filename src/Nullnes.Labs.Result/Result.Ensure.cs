using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    [Pure]
    public Result<TSuccess, TError> Ensure(
        Func<TSuccess, bool> predicate,
        Func<TSuccess, TError> errorFactory)
    {
        if (IsFailure) return this;
        return predicate(Value)
            ? this // Doesn't affect the current flow.
            : Result.Failure<TSuccess, TError>(errorFactory(Value)); // Fails by creating the new error based on the evaluated value.
    }

    [Pure]
    public Result<TSuccess, TError> Ensure(
        Func<TSuccess, bool> predicate,
        TError errorInstance)
    {
        if (IsFailure) return this;
        return predicate(Value)
            ? this // Doesn't affect the current flow.
            : Result.Failure<TSuccess, TError>(errorInstance); // Fails by using a given error.
    }
}

public static partial class AsyncResultExtensions
{
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
}