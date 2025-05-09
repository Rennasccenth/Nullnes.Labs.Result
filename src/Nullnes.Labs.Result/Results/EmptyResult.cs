using System.Diagnostics.Contracts;

namespace Nullnes.Labs.Result.Results;

public sealed record EmptyResult<TError> : Result<None, TError>
{
    private EmptyResult() : base(None.Value) { }
    private EmptyResult(TError errorInstance) : base(error: errorInstance) { }
    private static EmptyResult<TError> Success() => new();
    private static EmptyResult<TError> Failure(TError errorInstance) => new(errorInstance);

    public static implicit operator EmptyResult<TError>(None _) => Success();
    public static implicit operator EmptyResult<TError>(TError error) => Failure(error);

    // [Pure]
    // public EmptyResult<TError> Bind(Func<EmptyResult<TError>> onSuccess) => IsFailure 
    //     ? Error 
    //     : onSuccess();
    //
    // [Pure]
    // public EmptyResult<TError> Bind<TException>(Func<EmptyResult<TError>> onSuccess,
    //     Func<TException, EmptyResult<TError>> mapExceptionToError) where TException : Exception
    // {
    //     if (IsFailure) return Error;
    //     try
    //     {
    //         onSuccess();
    //         return this;
    //     }
    //     catch (TException e)
    //     {
    //         return mapExceptionToError(e);
    //     }
    // }
    //
    // [Pure]
    // public Result<TSuccess, TError> Bind<TSuccess>(Func<Result<TSuccess, TError>> onSuccess) => IsFailure 
    //     ? Error 
    //     : onSuccess();

    // Test Usage
    // [Pure]
    // public Result<TSuccess, TError> Bind<TSuccess, TException>(Func<Result<TSuccess, TError>> onSuccess,
    //     Func<TException, TError> mapExceptionToError) where TException : Exception
    // {
    //     if (IsFailure) return Error;
    //     try
    //     {
    //         return onSuccess();
    //     }
    //     catch (TException e)
    //     {
    //         return mapExceptionToError(e);
    //     }
    // }
    //
    // [Pure]
    // public Task<EmptyResult<TError>> Bind(Func<Task<EmptyResult<TError>>> onSuccessAsync) => IsFailure 
    //     ? Task.FromResult<EmptyResult<TError>>(Error) 
    //     : onSuccessAsync();

    // Test Usage
    // [Pure]
    // public Task<EmptyResult<TError>> Bind<TException>(Func<Task<EmptyResult<TError>>> onSuccessAsync,
    //     Func<TException, TError> mapExceptionToError) where TException : Exception
    // {
    //     if (IsFailure) return Task.FromResult<EmptyResult<TError>>(Error);
    //     try
    //     {
    //         return onSuccessAsync();
    //     }
    //     catch (TException e)
    //     {
    //         return Task.FromResult<EmptyResult<TError>>(mapExceptionToError(e));
    //     }
    // }
    // [Pure]
    // public Task<Result<TSuccess, TError>> Bind<TSuccess>(Func<Task<Result<TSuccess, TError>>> onSuccessAsync)
    //     => IsFailure 
    //         ? Task.FromResult<Result<TSuccess, TError>>(Error) 
    //         : onSuccessAsync();
    //
    // [Pure]
    // public Task<Result<TSuccess, TError>> Bind<TSuccess, TException>(
    //     Func<Task<Result<TSuccess, TError>>> onSuccessAsync,
    //     Func<TException, TError> mapExceptionToError) where TException : Exception
    // {
    //     if (IsFailure) return Task.FromResult<Result<TSuccess, TError>>(Error);
    //     try
    //     {
    //         return onSuccessAsync();
    //     }
    //     catch (TException e)
    //     {
    //         return Task.FromResult<Result<TSuccess, TError>>(mapExceptionToError(e));
    //     }
    // }
    //
    // [Pure]
    // public TOutput Match<TOutput>(
    //     Func<TOutput> onSuccess,
    //     Func<TError, TOutput> onError) =>
    //     IsSuccess
    //         ? onSuccess() 
    //         : onError(Error);
    //
    // [Pure]
    // public Task<TOutput> Match<TOutput>(
    //     Func<Task<TOutput>> onSuccessAsync,
    //     Func<TError, TOutput> onError) =>
    //     IsSuccess 
    //         ? onSuccessAsync() 
    //         : Task.FromResult(onError(Error));
    //
    // [Pure]
    // public Task<TOutput> Match<TOutput>(
    //     Func<TOutput> onSuccess,
    //     Func<TError, Task<TOutput>> onErrorAsync) =>
    //     IsSuccess
    //         ? Task.FromResult(onSuccess()) 
    //         : onErrorAsync(Error);
    //
    // [Pure]
    // public Task<TOutput> Match<TOutput>(
    //     Func<Task<TOutput>> onSuccessAsync,
    //     Func<TError, Task<TOutput>> onErrorAsync) =>
    //     IsSuccess
    //         ? onSuccessAsync() 
    //         : onErrorAsync(Error);

    [Pure]
    public new async Task<Result<TOutput, TError>> Map<TOutput>(Func<Task<TOutput>> asynchronousMapping)
        => IsSuccess ? await asynchronousMapping() : Error;

    [Pure]
    public new Result<TOutput, TError> Map<TOutput>(Func<TOutput> mapping)
        => IsSuccess ? mapping() : Error;

    [Pure]
    public new EmptyResult<TError> Tap(Action action)
    {
        if (IsFailure) return Error;
        action();
        return this;
    }
}