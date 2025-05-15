using System.Text;

namespace Nullnes.Labs.Result.Errors;

public sealed record InvalidArgumentError : BaseError
{
    public override string Type => "Invalid Argument";

    public override string Message => Reason is null
        ? new StringBuilder().Append("Invalid argument encountered, ")
            .Append(ArgumentName)
            .Append(" isn't a valid value.")
            .ToString()
        : new StringBuilder().Append("Invalid argument encountered, ")
            .Append(ArgumentName)
            .Append(" is invalid ")
            .Append(Reason)
            .Append('.')
            .ToString();

    private string ArgumentName { get; }
    private string? Reason { get; }

    internal InvalidArgumentError(string argumentName, string? reason = null)
    {
        ArgumentName = argumentName;

        if (reason is null) return;

        string loweredReason = reason
            .ToLowerInvariant()
            .TrimEnd('.')
            .TrimStart(' ');

        Reason = loweredReason.StartsWith("because", StringComparison.InvariantCulture)
            ? loweredReason
            : "because " + loweredReason;
    }
}