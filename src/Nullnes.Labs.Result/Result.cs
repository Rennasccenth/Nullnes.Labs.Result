using System.Diagnostics.Contracts;
using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result;

/// <summary>
/// Represents a result that can either be a success containing a value of type <typeparamref name="TSuccess"/>
/// or a failure containing an error of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public sealed partial record Result<TSuccess, TError> 
    where TError : class, IError
{
    private readonly TSuccess? _successValue;
    private readonly TError? _error;

    /// <summary>
    /// Unsafe access to the success value. Throws an <see cref="InvalidOperationException"/> if the result
    /// is not a success.
    /// </summary>
    /// <exception cref="InvalidOperationException">In case the result is not a success</exception>
    private TSuccess Value => IsSuccess 
        ? _successValue! 
        : throw new InvalidOperationException("The result is not a Success");

    /// <summary>
    /// Unsafe access to the error value. Throws an <see cref="InvalidOperationException"/> if the result
    /// is not a failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">In case the result is not a failure</exception>
    private TError Error => IsFailure 
        ? _error!
        : throw new InvalidOperationException("The result is a Success, not an Error");

    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    [Pure] 
    private bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents a failure.
    /// </summary>
    [Pure]
    private bool IsFailure => !IsSuccess;

    internal Result(TError error)
    { 
        _error = error;
        IsSuccess = false;
    }

    internal Result(TSuccess successData)
    {
        if (successData is TError)
        {
            throw new ArgumentException(
                $"Cannot construct a successful result by using an {nameof(TError)} instance.", nameof(successData));
        }
        _successValue = successData;
        IsSuccess = true;
    }

    /// <summary>
    /// Implicitly converts a <typeparamref name="TSuccess"/> value to a successful <see cref="Result{TSuccess, TError}"/>.
    /// </summary>
    /// <param name="data">The success value.</param>
    [Pure] 
    public static implicit operator Result<TSuccess, TError>(TSuccess data) => Result.Success<TSuccess, TError>(data);

    /// <summary>
    /// Implicitly converts a <typeparamref name="TError"/> value to a failed <see cref="Result{TSuccess, TError}"/>.
    /// </summary>
    /// <param name="error">The error value.</param>
    [Pure]
    public static implicit operator Result<TSuccess, TError>(TError error) => Result.Failure<TSuccess, TError>(error);

    [Pure] public override string? ToString() => IsSuccess ? Value?.ToString() : Error.ToString();
}

/// <summary>
/// Static non-generic factory for constructing <see cref="Result{TSuccess, TError}"/> instances.
/// </summary>
public static class Result
{
    /// <summary>
    /// Creates a successful <see cref="Result{TSuccess, TError}"/> instance.
    /// </summary>
    /// <param name="successData">Data representing the success.</param>
    /// <returns>A successful <see cref="Result{TSuccess, TError}"/> instance.</returns>
    // [Pure]
    public static Result<TSuccess, TError> Success<TSuccess, TError>(TSuccess successData) where TError : class, IError 
        => new(successData);

    /// <summary>
    /// Creates a failed <see cref="Result{TSuccess, TError}"/> instance.
    /// </summary>
    /// <param name="errorInstance">Data representing the error.</param>
    /// <returns>A failed <see cref="Result{TSuccess, TError}"/> instance.</returns>
    [Pure]
    public static Result<TSuccess, TError> Failure<TSuccess, TError>(TError errorInstance) where TError : class, IError
        => new(errorInstance);
}