using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

/// <summary>
/// Represents a result that can either be a success containing a value of type <typeparamref name="TSuccess"/>
/// or a failure containing an error of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public sealed record Result<TSuccess, TError> 
    where TError : class, IError
{
    private readonly TSuccess? _successValue;
    private readonly TError? _error;

    [Pure]
    private TSuccess Value => IsSuccess 
        ? _successValue! 
        : throw new InvalidOperationException("The result is not a Success");

    [Pure]
    private TError Error => IsFailure 
        ? _error!
        : throw new InvalidOperationException("The result is a Success, not an Error");

    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    private bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents a failure.
    /// </summary>
    private bool IsFailure => !IsSuccess;

    private Result(TError error)
    { 
        _error = error;
        IsSuccess = false;
    }

    private Result(TSuccess successData)
    {
        if (successData is TError)
        {
            throw new ArgumentException(
                $"Cannot construct a successful result by using an {nameof(TError)} instance.", nameof(successData));
        }
        _successValue = successData;
        IsSuccess = true;
    }

    [Pure]
    private static Result<TSuccess, TError> Success(TSuccess successData) => new(successData);
    [Pure]
    private static Result<TSuccess, TError> Failure(TError errorInstance) => new(errorInstance);

    /// <summary>
    /// Implicitly converts a <typeparamref name="TSuccess"/> value to a successful <see cref="Result{TSuccess, TError}"/>.
    /// </summary>
    /// <param name="data">The success value.</param>
    [Pure] 
    public static implicit operator Result<TSuccess, TError>(TSuccess data) => Success(data);

    /// <summary>
    /// Implicitly converts a <typeparamref name="TError"/> value to a failed <see cref="Result{TSuccess, TError}"/>.
    /// </summary>
    /// <param name="error">The error value.</param>
    [Pure]
    public static implicit operator Result<TSuccess, TError>(TError error) => Failure(error);

    [Pure] public override string? ToString() => IsSuccess ? Value?.ToString() : Error.ToString();

    # region Bind
    [Pure]
    public Result<TOutput, TError> Bind<TOutput>(Func<TSuccess, Result<TOutput, TError>> binder)
        => IsSuccess ? binder(Value) : Result<TOutput, TError>.Failure(Error);

    [Pure]
    public Result<TOutput, TError> Bind<TOutput>(Func<Result<TOutput, TError>> binder)
        => IsSuccess ? binder() : Result<TOutput, TError>.Failure(Error); 

    [Pure]
    public Task<Result<TOutput, TError>> Bind<TOutput>(Func<TSuccess, Task<Result<TOutput, TError>>> asyncBinder)
        => IsSuccess ? asyncBinder(Value) : Task.FromResult(Result<TOutput, TError>.Failure(Error));

    [Pure]
    public Task<Result<TOutput, TError>> Bind<TOutput>(Func<Task<Result<TOutput, TError>>> asyncBinder)
        => IsSuccess ? asyncBinder() : Task.FromResult(Result<TOutput, TError>.Failure(Error));

    # endregion

    # region Match
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
    # endregion

    # region Map
    [Pure]
    public Result<TOutput, TError> Map<TOutput>(Func<TSuccess, TOutput> mapper) =>
        IsSuccess
            ? Result<TOutput, TError>.Success(mapper(Value)) 
            : Result<TOutput, TError>.Failure(Error);

    [Pure]
    public async Task<Result<TOutput, TError>> Map<TOutput>(Func<TSuccess, Task<TOutput>> asyncMapper)
        => IsSuccess
            ? Result<TOutput, TError>.Success(await asyncMapper(Value)) 
            : Result<TOutput, TError>.Failure(Error);
    # endregion

    # region Map Error
    public Result<TSuccess, TNewError> MapError<TNewError>(Func<TError, TNewError> errorMapper) where TNewError : class, IError
    {
        return IsSuccess 
            ? Result<TSuccess, TNewError>.Success(Value) 
            : Result<TSuccess, TNewError>.Failure(errorMapper(Error));
    }
    # endregion

    # region OnSuccess
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
    # endregion

    # region OnFailure
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
    # endregion

    # region Ensure

    [Pure]
    public Result<TSuccess, TError> Ensure(
        Func<TSuccess, bool> predicate,
        Func<TSuccess, TError> errorFactory)
    {
        if (IsFailure) return Failure(Error);
        return predicate(Value)
            ? this // Doesn't affect the current flow.
            : Failure(errorFactory(Value)); // Fails by creating the new error based on the evaluated value.
    }

    [Pure]
    public Result<TSuccess, TError> Ensure(
        Func<TSuccess, bool> predicate,
        TError errorInstance)
    {
        if (IsFailure) return Failure(Error);
        return predicate(Value)
            ? this // Doesn't affect the current flow.
            : Failure(errorInstance); // Fails by using a given error.
    }
    #endregion
}