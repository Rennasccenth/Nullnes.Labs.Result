namespace Nullnes.Labs.Result.Results.Errors;

public sealed record PermissionDeniedError : BaseError
{
    public override string Type => "Permission Denied";
    public override string Message => "You're not allowed to access this resource.";
    public string[] Reasons { get; } = [];

    internal PermissionDeniedError(params string[] reasons)
    {
        Reasons = reasons;
    }

    internal PermissionDeniedError(IEnumerable<string> reasons)
    {
        Reasons = [..reasons];
    }
}