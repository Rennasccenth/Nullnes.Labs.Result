namespace Nullnes.Labs.Result.Results.Errors;

public sealed record UnprocessableError : BaseError
{
    public override string Type => "Unprocessable Error";
    public override string Message => "Cannot process this.";

    internal UnprocessableError() { }
}