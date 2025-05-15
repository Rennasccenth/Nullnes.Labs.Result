namespace Nullnes.Labs.Result.Errors;

public sealed record UnprocessableError : BaseError
{
    public override string Type => "Unprocessable Error";
    public override string Message => "Cannot process this.";

    internal UnprocessableError() { }
}