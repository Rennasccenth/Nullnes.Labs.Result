using System.Text;

namespace Nullnes.Labs.Result.Results.Errors;

public sealed record NotFoundError : BaseError
{
    public override string Type => "Not Found";

    public override string Message
    {
        get
        {
            if (ResourceName is null) return CustomMessage;

            StringBuilder stringBuilder = new();
            stringBuilder
                .Append("The related ")
                .Append(ResourceName);

            if (ResourceIdentifier is null)
            {
                return stringBuilder
                    .Append(" wasn't found in the system.")
                    .ToString();
            }

            return stringBuilder
                    .Append(" with ")
                    .Append(ResourceIdentifier)
                    .Append(" identifier wasn't found in the system.")
                    .ToString();    
        }
    }

    private string CustomMessage { get; init; } = string.Empty;
    private ResourceName? ResourceName { get; }
    private ResourceIdentifier? ResourceIdentifier { get; }
    
    internal NotFoundError(ResourceName resourceName, ResourceIdentifier resourceIdentifier)
    {
        ResourceName = resourceName;
        ResourceIdentifier = resourceIdentifier;
    }

    internal NotFoundError(ResourceName resourceName)
    {
        ResourceName = resourceName;
    }

    internal NotFoundError(string message)
    {
        CustomMessage = message;
    }
}