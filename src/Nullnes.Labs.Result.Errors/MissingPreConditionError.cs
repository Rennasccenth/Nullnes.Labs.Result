namespace Nullnes.Labs.Result.Errors;

public sealed record MissingPreConditionError : BaseError
{
    public override string Type => "Missing Pre-condition";
    public override string Message => "You need to fill an precondition before trying to executing this action.";

    public string[] MissingSteps { get; } = [];

    internal MissingPreConditionError(params string[] missingSteps)
    {
        MissingSteps = missingSteps;
    }

    internal MissingPreConditionError(IEnumerable<string> missingSteps)
    {
        MissingSteps = [..missingSteps];
    }
}