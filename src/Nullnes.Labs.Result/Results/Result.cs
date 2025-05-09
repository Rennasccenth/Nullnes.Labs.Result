using System.Diagnostics.Contracts;

namespace Nullnes.Labs.Result.Results;

/// <summary>
/// Represents a result that can either be a success containing a value of type <typeparamref name="TSuccess"/>
/// or a failure containing an error of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public record Result<TSuccess, TError>
{
    private readonly TSuccess? _successValue;
    private readonly TError? _error;

    [Pure]
    private TSuccess Value => IsSuccess 
        ? _successValue! 
        : throw new InvalidOperationException("The result is not a Success");

    [Pure]
    protected TError Error => IsFailure 
        ? _error!
        : throw new InvalidOperationException("The result is a Success, not an Error");

    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    protected bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents a failure.
    /// </summary>
    protected bool IsFailure => !IsSuccess;

    protected Result(TError error)
    { 
        _error = error;
        IsSuccess = false;
    }

    protected Result(TSuccess successData)
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

    [Pure] public override string? ToString() => IsSuccess ? Value?.ToString() : Error?.ToString();

    [Pure]
    public Result<TSuccess, TError> Bind(Func<TSuccess, Result<TSuccess, TError>> onSuccess)
        => IsFailure 
            ? Error 
            : onSuccess(Value);

    // [Pure]
    // public Result<TSuccess, TError> Bind(Func<Result<TSuccess, TError>> onSuccessAction)
    //     => IsFailure 
    //         ? Error 
    //         : onSuccessAction();
    
    [Pure]
    public Task<Result<TSuccess, TError>> Bind(Func<TSuccess, Task<Result<TSuccess, TError>>> onSuccess)
        => IsFailure 
            ? Task.FromResult<Result<TSuccess, TError>>(Error) 
            : onSuccess(Value);

    // Match Section [USING Success Value]
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
    
    // Match Section [USING Success Value]
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
    
    // Map Section [USING Success Value]
    [Pure]
    public Result<TOutput, TError> Map<TOutput>(
        Func<TSuccess, TOutput> mapping)
    {
        if (IsFailure) return Error;
        return mapping(Value);
    }

    [Pure]
    public async Task<Result<TOutput, TError>> Map<TOutput>(
        Func<TSuccess, Task<TOutput>> asynchronousMapping)
    {
        if (IsFailure) return Error;
        return await asynchronousMapping(Value);
    }

    // Map Section [NOT USING Success Value]
    [Pure]
    public Result<TOutput, TError> Map<TOutput>(
        Func<TOutput> mapping)
    {
        if (IsFailure) return Error;
        return mapping();
    }
    [Pure]
    public async Task<Result<TOutput, TError>> Map<TOutput>(
        Func<Task<TOutput>> asynchronousMapping)
    {
        if (IsFailure) return Error;
        return await asynchronousMapping();
    }

    // Tap Section
    [Pure]
    public Result<TSuccess, TError> Tap(Action<TSuccess> callable)
    {
        if (IsFailure) return Error;
        callable(Value);
        return this;
    }

    [Pure]
    public Result<TSuccess, TError> Tap(Action callable)
    {
        if (IsFailure) return Error;
        callable();
        return this;
    }
}