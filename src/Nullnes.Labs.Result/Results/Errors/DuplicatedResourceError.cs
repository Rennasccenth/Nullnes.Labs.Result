using System.Text;

namespace Nullnes.Labs.Result.Results.Errors;

public sealed record DuplicatedResourceError : BaseError
{
    public override string Type => "Duplicated Resource";

    public override string Message
    {
        get
        {
            StringBuilder stringBuilder = new();
            stringBuilder
                .Append("There's already an existing ")
                .Append(ResourceName);

            if (ResourceIdentifier is null)
                return stringBuilder
                        .Append("in the system.")
                        .ToString();

            return stringBuilder
                .Append(" with ")
                .Append(ResourceIdentifier)
                .Append(" identifier in the system.")
                .ToString();
        }
    }

    private ResourceName ResourceName { get; }
    private ResourceIdentifier? ResourceIdentifier { get; }
    internal DuplicatedResourceError(ResourceName resourceName)
    {
        ResourceName = resourceName;
    }
    internal DuplicatedResourceError(ResourceName resourceName, ResourceIdentifier resourceIdentifier)
    {
        ResourceName = resourceName;
        ResourceIdentifier = resourceIdentifier;
    }
}